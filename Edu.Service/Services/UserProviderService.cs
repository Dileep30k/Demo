using Core.Repository.Models;
using Core.Utility.Utils;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Edu.Service.Interfaces;
using Core.Repository.Constants;
using Edu.Abstraction.Enums;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using System.Security.Principal;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Edu.Service.Services
{
    public class UserProviderService : IUserProviderService
    {
        private readonly IHttpContextAccessor _context;

        public UserProviderService(IHttpContextAccessor context)
        {
            _context = context;
        }

        public UserClaim UserClaim => GetUserClaim();
        public bool IsAuthenticated() => _context.HttpContext.User.Identity == null ? false : _context.HttpContext.User.Identity.IsAuthenticated;



        public void RemoveSessionUser()
        {
            _context.HttpContext.User = new ClaimsPrincipal();
        }

        private UserClaim GetUserClaim()
        {
            var userClaims = new UserClaim();

            var name = _context.HttpContext.User.Claims.FirstOrDefault(i => i.Type == CustomClaimTypes.Username);
            if (name != null) { userClaims.Username = name.Value; }

            var msilUserId = _context.HttpContext.User.Claims.FirstOrDefault(i => i.Type == CustomClaimTypes.MsilUserId);
            if (msilUserId != null) { userClaims.MsilUserId = Convert.ToInt64(msilUserId.Value); }

            var userId = _context.HttpContext.User.Claims.FirstOrDefault(i => i.Type == CustomClaimTypes.UserId);
            if (userId != null && !string.IsNullOrEmpty(userId.Value)) { userClaims.UserId = Convert.ToInt64(userId.Value); }

            var isAdmin = _context.HttpContext.User.Claims.FirstOrDefault(i => i.Type == CustomClaimTypes.IsAdmin);
            if (isAdmin != null && !string.IsNullOrEmpty(isAdmin.Value)) { userClaims.IsAdmin = Convert.ToBoolean(isAdmin.Value); }

            var sessionId = _context.HttpContext.User.Claims.FirstOrDefault(i => i.Type == CustomClaimTypes.SessionId);
            if (sessionId != null) { userClaims.SessionId = sessionId.Value; }

            var roleIds = _context.HttpContext.User.Claims.FirstOrDefault(i => i.Type == CustomClaimTypes.RoleIds);
            if (roleIds != null && !string.IsNullOrEmpty(roleIds.Value)) { userClaims.RoleIds = roleIds.Value.Split(',').Select(long.Parse).ToList(); }

            var departmentId = _context.HttpContext.User.Claims.FirstOrDefault(i => i.Type == CustomClaimTypes.DepartmentId);
            if (departmentId != null && !string.IsNullOrEmpty(departmentId.Value)) { userClaims.DepartmentId = Convert.ToInt64(departmentId.Value); }

            var approver = Roles.Approver.GetHashCode();
            var employee = Roles.Employee.GetHashCode();
            var gts = Roles.GTS.GetHashCode();

            userClaims.IsApprover = userClaims.RoleIds.Contains(approver);
            userClaims.IsEmployee = userClaims.RoleIds.Contains(employee);
            userClaims.IsGts = userClaims.RoleIds.Contains(gts);

            return userClaims;
        }

    }
}
