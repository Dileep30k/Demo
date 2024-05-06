using Core.Repository.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Edu.Abstraction.ComplexModels;
using Edu.Abstraction.Enums;
using Edu.Abstraction.Models;
using Edu.Service.Interfaces;
using Edu.Web.Models;
using Edu.Service.Services;

namespace Edu.Web.Controllers
{
    public class SchedularController : BaseController
    {
        private readonly IUserProviderService _userProviderService;

        public SchedularController(
           IUserProviderService userProviderService
        )
        {
            _userProviderService = userProviderService;
        }

        public async Task<IActionResult> Index()
        {
            if (!AuthorizeVertical()) { return AccessDeniedView(); }

            return View();
        }

        private bool AuthorizeVertical()
        {
            return _userProviderService.UserClaim.IsAdmin || _userProviderService.UserClaim.IsGts;
        }
    }
}
