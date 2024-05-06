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
    public class SettingController : BaseController
    {
        private readonly ISettingService _settingService;
        private readonly IUserProviderService _userProviderService;

        public SettingController(
           ISettingService settingService,
           IUserProviderService userProviderService
        )
        {
            _settingService = settingService;
            _userProviderService = userProviderService;
        }

        public async Task<IActionResult> Index()
        {
            if (!AuthorizeSetting()) { return AccessDeniedView(); }

            return View();
        }

        public async Task<IActionResult> Manage(long id = 0)
        {
            if (!AuthorizeSetting()) { return AccessDeniedView(); }

            var model = new Setting();
            if (id > 0)
            {
                var settingResult = await _settingService.GetSettingById(id);
                if (settingResult.Success)
                {
                    model = settingResult.Data;
                }
            }

            await SetComboBoxes(model);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(Setting model)
        {
            ResponseModel settingResult;
            if (model.SettingId == 0)
            {
                settingResult = await _settingService.CreateSetting(model);
            }
            else
            {
                settingResult = await _settingService.UpdateSetting(model);
            }

            if (settingResult.Success)
            {
                SetNotification($"Setting saved successfully!", NotificationTypes.Success, "Setting");
                return RedirectToAction("Index", "Setting");
            }

            SetNotification(settingResult.Message, NotificationTypes.Error, "Setting");
            await SetComboBoxes(model);
            return View(model);
        }

        public async Task<IActionResult> GetSettings()
        {
            try
            {
                dynamic filters = new ExpandoObject();
                dynamic grid = GetGridPagination(filters);
                var result = await _settingService.GetSettingPaged(grid.pagination);
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

        private async Task SetComboBoxes(Setting setting)
        {
        }

        private bool AuthorizeSetting()
        {
            return _userProviderService.UserClaim.IsAdmin;
        }
    }
}
