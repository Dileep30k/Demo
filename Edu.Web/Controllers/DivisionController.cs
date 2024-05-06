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
    public class DivisionController : BaseController
    {
        private readonly IDivisionService _divisionService;
        private readonly IUserProviderService _userProviderService;

        public DivisionController(
           IDivisionService divisionService,
           IUserProviderService userProviderService
        )
        {
            _divisionService = divisionService;
            _userProviderService = userProviderService;
        }

        public async Task<IActionResult> Index()
        {
            if (!AuthorizeDivision()) { return AccessDeniedView(); }

            return View();
        }

        public async Task<IActionResult> Manage(long id = 0)
        {
            if (!AuthorizeDivision()) { return AccessDeniedView(); }

            var model = new Division();
            if (id > 0)
            {
                var divisionResult = await _divisionService.GetDivisionById(id);
                if (divisionResult.Success)
                {
                    model = divisionResult.Data;
                }
            }

            await SetComboBoxes(model);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(Division model)
        {
            ResponseModel divisionResult;
            if (model.DivisionId == 0)
            {
                divisionResult = await _divisionService.CreateDivision(model);
            }
            else
            {
                divisionResult = await _divisionService.UpdateDivision(model);
            }

            if (divisionResult.Success)
            {
                SetNotification($"Division saved successfully!", NotificationTypes.Success, "Division");
                return RedirectToAction("Index", "Division");
            }

            SetNotification(divisionResult.Message, NotificationTypes.Error, "Division");
            await SetComboBoxes(model);
            return View(model);
        }

        public async Task<IActionResult> GetDivisions()
        {
            try
            {
                dynamic filters = new ExpandoObject();
                dynamic grid = GetGridPagination(filters);
                var result = await _divisionService.GetDivisionPaged(grid.pagination);
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

        private async Task SetComboBoxes(Division division)
        {
        }

        private bool AuthorizeDivision()
        {
            return _userProviderService.UserClaim.IsAdmin;
        }
    }
}
