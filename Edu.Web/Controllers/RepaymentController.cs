using Edu.Abstraction.ComplexModels;
using Edu.Service.Interfaces;
using Edu.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Edu.Web.Controllers
{
    public class RepaymentController : BaseController
    {
        private readonly ILogger<RepaymentController> _logger;
        private readonly IUserProviderService _userProviderService;

        public RepaymentController(
            ILogger<RepaymentController> logger,
            IUserProviderService userProviderService
        )
        {
            _logger = logger;
            _userProviderService = userProviderService;
        }

        public IActionResult Index()
        {
            if (!AuthorizeUser()) { return AccessDeniedView(); }
            return View();
        }
        private bool AuthorizeUser()
        {
            return _userProviderService.UserClaim.IsGts;
        }
    }
}