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

namespace Edu.Web.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IUserProviderService _userProviderService;
        private readonly IRoleService _roleService;
        private readonly IDivisionService _divisionService;
        private readonly IDepartmentService _departmentService;
        private readonly IDesignationService _designationService;
        private readonly ILocationService _locationService;
        private readonly IVerticalService _verticalService;

        public UserController(
           IUserService userService,
           IUserProviderService userProviderService,
           IRoleService roleService,
           IDivisionService divisionService,
           IDepartmentService departmentService,
           IDesignationService designationService,
           ILocationService locationService,
           IVerticalService verticalService
        )
        {
            _userService = userService;
            _userProviderService = userProviderService;
            _roleService = roleService;
            _divisionService = divisionService;
            _departmentService = departmentService;
            _designationService = designationService;
            _locationService = locationService;
            _verticalService = verticalService;
        }

        public async Task<IActionResult> Index()
        {
            if (!AuthorizeUser()) { return AccessDeniedView(); }

            return View();
        }

        public async Task<IActionResult> Manage(long id = 0)
        {
            if (!AuthorizeUser()) { return AccessDeniedView(); }

            var model = new User();
            if (id > 0)
            {
                var userResult = await _userService.GetUserById(id);
                if (userResult.Success)
                {
                    model = userResult.Data;
                }
            }

            await SetComboBoxes(model);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(User model)
        {
            ResponseModel userResult;
            if (model.UserId == 0)
            {
                userResult = await _userService.CreateUser(model);
            }
            else
            {
                userResult = await _userService.UpdateUser(model);
            }

            if (userResult.Success)
            {
                SetNotification($"User saved successfully!", NotificationTypes.Success, "User");
                return RedirectToAction("Index", "User");
            }

            SetNotification(userResult.Message, NotificationTypes.Error, "User");
            await SetComboBoxes(model);
            return View(model);
        }

        public async Task<IActionResult> GetUsers()
        {
            try
            {
                dynamic filters = new ExpandoObject();
                dynamic grid = GetGridPagination(filters);
                var result = await _userService.GetUserPaged(grid.pagination);
                var paged = result.Data as PagedList;
                return Ok(new
                {
                    draw = grid.draw,
                    recordsFiltered = paged.TotalCount,
                    recordsTotal = paged.TotalCount,
                    data = paged.Data
                });
            }
            catch (Exception) { throw; }
        }

        private async Task SetComboBoxes(User user)
        {
            ViewBag.Verticals = GetSelectList(
                          await _verticalService.GetDropdwon(),
                          "Select Vertical",
                          user.VerticalId.HasValue ? new List<long> { user.VerticalId.Value } : null
                      );

            ViewBag.Divisions = GetSelectList(
                         await _divisionService.GetDropdwon(),
                         "Select Divisions",
                          user.DivisionId.HasValue ? new List<long> { user.DivisionId.Value } : null
                     );

            ViewBag.Designations = GetSelectList(
                          await _designationService.GetDropdwon(),
                          "Select Designation",
                          user.DesignationId.HasValue ? new List<long> { user.DesignationId.Value } : null
                      );

            ViewBag.Departments = GetSelectList(
                          await _departmentService.GetDropdwon(),
                          "Select Department",
                          user.DepartmentId.HasValue ? new List<long> { user.DepartmentId.Value } : null
                      );

            ViewBag.Locations = GetSelectList(
                          await _locationService.GetDropdwon(),
                          "Select Location",
                          user.LocationId.HasValue ? new List<long> { user.LocationId.Value } : null
                      );
        }

        private bool AuthorizeUser()
        {
            return _userProviderService.UserClaim.IsAdmin;
        }
    }
}
