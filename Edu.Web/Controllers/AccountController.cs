using Core.Repository.Constants;
using Core.Repository.Models;
using Edu.Abstraction.ComplexModels;
using Edu.Abstraction.Enums;
using Edu.Abstraction.Models;
using Edu.Service.Interfaces;
using Edu.Service.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Edu.Web.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IUserLoginService _userLoginService;
        private readonly IUserProviderService _userProviderService;
        private readonly IRoleDesignationService _roleDesignationService;

        public AccountController(
            IConfiguration configuration,
            IUserService userService,
            IUserLoginService userLoginService,
            IUserProviderService userProviderService,
            IRoleDesignationService roleDesignationService
        )
        {
            _configuration = configuration;
            _userService = userService;
            _userProviderService = userProviderService;
            _userLoginService = userLoginService;
            _roleDesignationService = roleDesignationService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string returnUrl = "")
        {
            if (returnUrl == "exps")
            {
                ViewBag.ExpiredSession = true;
                returnUrl = "";
            }
            await ClearSession();
            if (returnUrl == "/Logout")
            {
                returnUrl = "";
            }
            return View(new LoginModel { ReturnUrl = returnUrl });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Index(LoginModel loginModel)
        {
            if (!ModelState.IsValid) return View(loginModel);

            if (string.IsNullOrEmpty(loginModel.Password))
            {
                SetNotification("Please enter password.", NotificationTypes.Error, "Login");
                ModelState.AddModelError("", "Please enter password.");
                return View(loginModel);
            }

            var userResult = await _userService.AuthenticateUser(loginModel);

            if (userResult.Success)
            {
                var user = userResult.Data as User;

                var userLogin = new ResponseModel { Success = false };

                if (bool.Parse(_configuration.GetSection("SingleUserLogin").Value))
                {
                    userLogin = await _userLoginService.GetUserLoginByMsilUserId(loginModel.MsilUserId);
                }

                var roles = await _roleDesignationService.GetRolesByDesignationId(user.DesignationId);

                var claims = new[]
                {
                    new Claim(CustomClaimTypes.UserId, user.UserId.ToString()),
                    new Claim(CustomClaimTypes.Username, $"{user.FirstName} {user.LastName}"),
                    new Claim(CustomClaimTypes.RoleIds, string.Join(",", roles)),
                    new Claim(CustomClaimTypes.IsAdmin, user.IsAdmin.ToString()),
                    new Claim(CustomClaimTypes.MsilUserId, user.MsilUserId.ToString()),
                    new Claim(CustomClaimTypes.SessionId, userLogin.Success ? (userLogin.Data as UserLogin).SessionId : ""),
                    new Claim(CustomClaimTypes.DepartmentId, user.DepartmentId.HasValue ? user.DepartmentId.ToString() : ""),
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), new AuthenticationProperties
                {
                    IsPersistent = false
                });

                if (!string.IsNullOrEmpty(loginModel.ReturnUrl) && loginModel.ReturnUrl != "/Logout" && Url.IsLocalUrl(loginModel.ReturnUrl))
                {
                    return Redirect(loginModel.ReturnUrl);
                }
                else
                {
                    var gts = Roles.GTS.GetHashCode();
                    if (roles.Contains(gts))
                    {
                        return RedirectToAction("Index", "Dashboard");
                    }
                    return RedirectToAction("Employee", "Dashboard");
                }
            }
            else if (userResult.StatusCode == StatusCodes.Status403Forbidden)
            {
                loginModel.ForceLogin = true;
            }

            SetNotification(userResult.Message, NotificationTypes.Error, "Login");
            ModelState.AddModelError("", userResult.Message);
            return View(loginModel);
        }

        [HttpGet]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await ClearSession();
            return RedirectToAction("Index", "Account");
        }

        private async Task ClearSession()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddDays(-1)
            });
            //foreach (var cookie in HttpContext.Request.Cookies)
            //{
            //    Response.Cookies.Delete(cookie.Key);
            //}
            _userProviderService.RemoveSessionUser();
        }
    }
}
