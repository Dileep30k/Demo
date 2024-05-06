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
    public class DesignationController : BaseController
    {
        private readonly IDesignationService _designationService;
        private readonly IUserProviderService _userProviderService;

        public DesignationController(
           IDesignationService designationService,
           IUserProviderService userProviderService
        )
        {
            _designationService = designationService;
            _userProviderService = userProviderService;
        }

        public async Task<IActionResult> Index()
        {
            if (!AuthorizeDesignation()) { return AccessDeniedView(); }

            return View();
        }

        public async Task<IActionResult> Manage(long id = 0)
        {
            if (!AuthorizeDesignation()) { return AccessDeniedView(); }

            var model = new Designation();
            if (id > 0)
            {
                var designationResult = await _designationService.GetDesignationById(id);
                if (designationResult.Success)
                {
                    model = designationResult.Data;
                }
            }

            await SetComboBoxes(model);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(Designation model)
        {
            ResponseModel designationResult;
            if (model.DesignationId == 0)
            {
                designationResult = await _designationService.CreateDesignation(model);
            }
            else
            {
                designationResult = await _designationService.UpdateDesignation(model);
            }

            if (designationResult.Success)
            {
                SetNotification($"Designation saved successfully!", NotificationTypes.Success, "Designation");
                return RedirectToAction("Index", "Designation");
            }

            SetNotification(designationResult.Message, NotificationTypes.Error, "Designation");
            await SetComboBoxes(model);
            return View(model);
        }

        public async Task<IActionResult> GetDesignations()
        {
            try
            {
                dynamic filters = new ExpandoObject();
                dynamic grid = GetGridPagination(filters);
                var result = await _designationService.GetDesignationPaged(grid.pagination);
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

        private async Task SetComboBoxes(Designation designation)
        {
        }

        private bool AuthorizeDesignation()
        {
            return _userProviderService.UserClaim.IsAdmin;
        }
    }
}
