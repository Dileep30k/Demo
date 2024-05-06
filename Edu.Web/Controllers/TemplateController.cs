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
    public class TemplateController : BaseController
    {
        private readonly ISchemeService _schemeService;
        private readonly IIntakeService _intakeService;
        private readonly IIntakeTemplateService _intakeTemplateService;
        private readonly IUserProviderService _userProviderService;

        public TemplateController(
           ISchemeService schemeService,
           IIntakeService intakeService,
           IIntakeTemplateService intakeTemplateService,
           IUserProviderService userProviderService
        )
        {
            _schemeService = schemeService;
            _intakeService = intakeService;
            _intakeTemplateService = intakeTemplateService;
            _userProviderService = userProviderService;
        }

        public async Task<IActionResult> Index()
        {
            if (!AuthorizeUser()) { return AccessDeniedView(); }

            await SetComboBoxes();
            return View();
        }

        public async Task<IActionResult> Manage(long id = 0)
        {
            if (!AuthorizeUser()) { return AccessDeniedView(); }

            var model = new IntakeTemplate();
            if (id > 0)
            {
                var templateResult = await _intakeTemplateService.GetIntakeTemplateById(id);
                if (templateResult.Success)
                {
                    model = templateResult.Data;
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(IntakeTemplate model)
        {
            ResponseModel templateResult;
            templateResult = await _intakeTemplateService.UpdateIntakeTemplate(model);

            if (templateResult.Success)
            {
                SetNotification($"Template saved successfully!", NotificationTypes.Success, "Template");
                return RedirectToAction("Index", "Template");
            }

            SetNotification(templateResult.Message, NotificationTypes.Error, "Template");
            return View(model);
        }

        public async Task<IActionResult> GetTemplates(
            long? schemeId = null,
            long? intakeId = null
        )
        {
            try
            {
                dynamic filters = new ExpandoObject();
                filters.schemeId = schemeId;
                filters.intakeId = intakeId;

                dynamic grid = GetGridPagination(filters);
                var result = await _intakeTemplateService.GetIntakeTemplatePaged(grid.pagination);
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

        public async Task<IActionResult> GetSchemeIntakes(long? schemeId)
        {
            return Ok(await _intakeService.GetDropdwon(schemeId: schemeId));
        }

        private async Task SetComboBoxes()
        {
            ViewBag.Schemes = GetSelectList(
                        await _schemeService.GetDropdwon(),
                        "All");

            ViewBag.Intakes = GetSelectList(
                        new List<DropdownModel>(),
                        "All");
        }

        private bool AuthorizeUser()
        {
            return _userProviderService.UserClaim.IsGts;
        }
    }
}
