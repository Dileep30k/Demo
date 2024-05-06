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
    public class DepartmentController : BaseController
    {
        private readonly IDepartmentService _departmentService;
        private readonly IUserProviderService _userProviderService;

        public DepartmentController(
           IDepartmentService departmentService,
           IUserProviderService userProviderService
        )
        {
            _departmentService = departmentService;
            _userProviderService = userProviderService;
        }

        public async Task<IActionResult> Index()
        {
            if (!AuthorizeDepartment()) { return AccessDeniedView(); }

            return View();
        }

        public async Task<IActionResult> Manage(long id = 0)
        {
            if (!AuthorizeDepartment()) { return AccessDeniedView(); }

            var model = new Department();
            if (id > 0)
            {
                var departmentResult = await _departmentService.GetDepartmentById(id);
                if (departmentResult.Success)
                {
                    model = departmentResult.Data;
                }
            }

            await SetComboBoxes(model);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(Department model)
        {
            ResponseModel departmentResult;
            if (model.DepartmentId == 0)
            {
                departmentResult = await _departmentService.CreateDepartment(model);
            }
            else
            {
                departmentResult = await _departmentService.UpdateDepartment(model);
            }

            if (departmentResult.Success)
            {
                SetNotification($"Department saved successfully!", NotificationTypes.Success, "Department");
                return RedirectToAction("Index", "Department");
            }

            SetNotification(departmentResult.Message, NotificationTypes.Error, "Department");
            await SetComboBoxes(model);
            return View(model);
        }

        public async Task<IActionResult> GetDepartments()
        {
            try
            {
                dynamic filters = new ExpandoObject();
                dynamic grid = GetGridPagination(filters);
                var result = await _departmentService.GetDepartmentPaged(grid.pagination);
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

        private async Task SetComboBoxes(Department department)
        {
        }

        private bool AuthorizeDepartment()
        {
            return _userProviderService.UserClaim.IsAdmin;
        }
    }
}
