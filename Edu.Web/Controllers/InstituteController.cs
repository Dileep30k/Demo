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
    public class InstituteController : BaseController
    {
        private readonly IInstituteService _instituteService;
        private readonly IUserProviderService _userProviderService;
        private readonly ISchemeService _schemeService;

        public InstituteController(
           IInstituteService instituteService,
            ISchemeService schemeService,
           IUserProviderService userProviderService
        )
        {
            _instituteService = instituteService;
            _userProviderService = userProviderService;
            _schemeService = schemeService;
        }

        public async Task<IActionResult> Index()
        {
            if (!AuthorizeInstitute()) { return AccessDeniedView(); }

            await SetComboBoxes();
            return View();
        }

        public async Task<IActionResult> Manage(long id = 0)
        {
            if (!AuthorizeInstitute()) { return AccessDeniedView(); }

            var model = new Institute();
            if (id > 0)
            {
                var instituteResult = await _instituteService.GetInstituteById(id);
                if (instituteResult.Success)
                {
                    model = instituteResult.Data;
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(Institute model)
        {
            ResponseModel instituteResult;
            if (model.InstituteId == 0)
            {
                instituteResult = await _instituteService.CreateInstitute(model);
            }
            else
            {
                instituteResult = await _instituteService.UpdateInstitute(model);
            }

            if (instituteResult.Success)
            {
                SetNotification($"Institute saved successfully!", NotificationTypes.Success, "Institute");
                return RedirectToAction("Index", "Institute");
            }

            SetNotification(instituteResult.Message, NotificationTypes.Error, "Institute");
            return View(model);
        }

        public async Task<IActionResult> GetInstitutes(
            string? searchValue = null,
            long? schemeId = null
        )
        {
            try
            {
                dynamic filters = new ExpandoObject();
                filters.searchValue = searchValue;
                filters.schemeId = schemeId;

                dynamic grid = GetGridPagination(filters);
                var result = await _instituteService.GetInstitutePaged(grid.pagination);
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

        private async Task SetComboBoxes()
        {
            ViewBag.Schemes = GetSelectList(
            await _schemeService.GetDropdwon(),
                         "Scheme: All");
        }

        private bool AuthorizeInstitute()
        {
            return _userProviderService.UserClaim.IsGts;
        }
    }
}
