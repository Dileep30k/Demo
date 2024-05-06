using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System;
using System.Threading.Tasks;
using Edu.Abstraction.Enums;
using Edu.Abstraction.Models;
using Edu.Service.Interfaces;
using System.Linq;
using Edu.Abstraction.ComplexModels;
using System.Reflection.Emit;
using Core.Repository.Models;
using System.Dynamic;
using Edu.Service.Services;

namespace Edu.Web.Controllers
{
    public class RoleController : BaseController
    {
        private readonly IRoleService _roleService;
        private readonly IRoleDesignationService _roleDesignationService;
        private readonly IUserProviderService _userProviderService;
        private readonly IDesignationService _designationService;

        public RoleController(
            IRoleService roleService,
            IRoleDesignationService roleDesignationService,
            IUserProviderService userProviderService,
            IDesignationService designationService

        )
        {
            _roleService = roleService;
            _roleDesignationService = roleDesignationService;
            _userProviderService = userProviderService;
            _designationService = designationService;
        }

        public async Task<IActionResult> Index()
        {
            if (!AuthorizeUser()) { return AccessDeniedView(); }
            return View();
        }

        public async Task<IActionResult> Manage(long id = 0)
        {
            if (!AuthorizeUser()) { return AccessDeniedView(); }

            var model = new Role { };
            if (id > 0)
            {
                var roleResult = await _roleService.GetRoleById(id);
                if (roleResult.Success)
                {
                    model = roleResult.Data;
                }
            }

            await SetComboBoxes();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(Role model)
        {
            ResponseModel roleResult;
            if (model.RoleId == 0)
            {
                roleResult = await _roleService.CreateRole(model);
            }
            else
            {
                roleResult = await _roleService.UpdateRole(model);
            }

            if (roleResult.Success)
            {
                SetNotification($"Role saved successfully!", NotificationTypes.Success, "Role");
                return RedirectToAction("Manage", "Role", new { id = roleResult.Data?.RoleId });
            }

            await SetComboBoxes();
            SetNotification(roleResult.Message, NotificationTypes.Error, "Role");
            return View(model);
        }

        public async Task<IActionResult> GetRoles()
        {
            try
            {
                dynamic filters = new ExpandoObject();
                dynamic grid = GetGridPagination(filters);
                var result = await _roleService.GetRolePaged(grid.pagination);
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

        public async Task<IActionResult> AddRoleDesignation(long roleId, long designationId)
        {
            return Ok(await _roleDesignationService.CreateRoleDesignation(new RoleDesignation
            {
                RoleId = roleId,
                DesignationId = designationId,
            }));
        }

        public async Task<IActionResult> RemoveRoleDesignation(long roleDesignationId)
        {
            return Ok(await _roleDesignationService.DeleteRoleDesignation(roleDesignationId));
        }

        public async Task<IActionResult> GetRoleDesignations(long roleId)
        {
            try
            {
                dynamic filters = new ExpandoObject();
                filters.roleId = roleId;

                var roleDesignationResult = await _roleDesignationService.GetRoleDesignationPaged(
                    new Pagination
                    {
                        Filters = filters
                    });

                var roleDesignationPaged = roleDesignationResult.Data as PagedList;
                return PartialView("_DesignationList", roleDesignationPaged.Data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task SetComboBoxes()
        {
            ViewBag.Designations = GetSelectList(
                          await _designationService.GetDropdwon(),
                          "Select Designation");
        }

        private bool AuthorizeUser()
        {
            return _userProviderService.UserClaim.IsAdmin;
        }
    }
}
