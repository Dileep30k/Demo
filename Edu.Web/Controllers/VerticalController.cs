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
    public class VerticalController : BaseController
    {
        private readonly IVerticalService _verticalService;
        private readonly IUserProviderService _userProviderService;

        public VerticalController(
           IVerticalService verticalService,
           IUserProviderService userProviderService
        )
        {
            _verticalService = verticalService;
            _userProviderService = userProviderService;
        }

        public async Task<IActionResult> Index()
        {
            if (!AuthorizeVertical()) { return AccessDeniedView(); }

            return View();
        }

        public async Task<IActionResult> Manage(long id = 0)
        {
            if (!AuthorizeVertical()) { return AccessDeniedView(); }

            var model = new Vertical();
            if (id > 0)
            {
                var verticalResult = await _verticalService.GetVerticalById(id);
                if (verticalResult.Success)
                {
                    model = verticalResult.Data;
                }
            }

            await SetComboBoxes(model);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(Vertical model)
        {
            ResponseModel verticalResult;
            if (model.VerticalId == 0)
            {
                verticalResult = await _verticalService.CreateVertical(model);
            }
            else
            {
                verticalResult = await _verticalService.UpdateVertical(model);
            }

            if (verticalResult.Success)
            {
                SetNotification($"Vertical saved successfully!", NotificationTypes.Success, "Vertical");
                return RedirectToAction("Index", "Vertical");
            }

            SetNotification(verticalResult.Message, NotificationTypes.Error, "Vertical");
            await SetComboBoxes(model);
            return View(model);
        }

        public async Task<IActionResult> GetVerticals()
        {
            try
            {
                dynamic filters = new ExpandoObject();
                dynamic grid = GetGridPagination(filters);
                var result = await _verticalService.GetVerticalPaged(grid.pagination);
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

        private async Task SetComboBoxes(Vertical vertical)
        {
        }

        private bool AuthorizeVertical()
        {
            return _userProviderService.UserClaim.IsAdmin;
        }
    }
}
