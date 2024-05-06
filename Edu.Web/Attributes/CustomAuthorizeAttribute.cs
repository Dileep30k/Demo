using Edu.Abstraction.Models;
using Edu.Service.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edu.Abstraction.Models;
using Edu.Service.Interfaces;
using System.Configuration;
using Core.Repository.Constants;

namespace Edu.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        IConfigurationBuilder builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        public CustomAuthorizeAttribute()
        {
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any())
                return;

            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                // it isn't needed to set unauthorized result 
                // as the base class already requires the user to be authenticated
                // this also makes redirect to a login page work properly
                // context.Result = new UnauthorizedResult();
                return;
            }

            IConfigurationRoot configuration = builder.Build();
            if (!bool.Parse(configuration.GetSection("SingleUserLogin").Value))
            {
                return;
            }

            if (!user.HasClaim(c => c.Type == CustomClaimTypes.SessionId) || !user.HasClaim(c => c.Type == CustomClaimTypes.MsilUserId))
            {
                context.Result = new ChallengeResult(CookieAuthenticationDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = "exps" });
                return;
            }

            var sessionId = user.Claims.First(c => c.Type == CustomClaimTypes.SessionId).Value;
            var msilUserId = user.Claims.First(c => c.Type == CustomClaimTypes.MsilUserId).Value;

            var userLoginService = context.HttpContext.RequestServices.GetService(typeof(IUserLoginService)) as IUserLoginService;
            var userLoginResult = userLoginService.GetUserLoginByMsilUserId(Convert.ToInt64(msilUserId)).GetAwaiter().GetResult();
            if (userLoginResult.Success)
            {
                var userLogin = userLoginResult.Data as UserLogin;
                if (userLogin.LogInExpireTime < DateTime.Now || userLogin.SessionId != sessionId)
                {
                    context.Result = new ChallengeResult(CookieAuthenticationDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = "exps" });
                    return;
                }
            }
            else
            {
                context.Result = new ChallengeResult(CookieAuthenticationDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = "exps" });
                return;
            }
        }
    }
}
