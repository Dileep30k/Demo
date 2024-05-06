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
    public class SchemeController : BaseController
    {
        private readonly ISchemeService _schemeService;
        private readonly ISchemeInstituteService _schemeInstituteService;
        private readonly IUserProviderService _userProviderService;
        private readonly IInstituteService _instituteService;
        private readonly IDurationTypeService _durationTypeService;

        public SchemeController(
            ISchemeService schemeService,
            ISchemeInstituteService schemeInstituteService,
            IUserProviderService userProviderService,
            IInstituteService instituteService,
            IDurationTypeService durationTypeService

        )
        {
            _schemeService = schemeService;
            _schemeInstituteService = schemeInstituteService;
            _userProviderService = userProviderService;
            _instituteService = instituteService;
            _durationTypeService = durationTypeService;
        }

        public async Task<IActionResult> Index()
        {
            if (!AuthorizeUser()) { return AccessDeniedView(); }

            ViewBag.Institutes = GetSelectList(
                          await _instituteService.GetDropdwon(),
                          "Institute: All");

            return View();
        }

        public async Task<IActionResult> Manage(long id = 0)
        {
            if (!AuthorizeUser()) { return AccessDeniedView(); }

            var model = new Scheme { };
            if (id > 0)
            {
                var schemeResult = await _schemeService.GetSchemeById(id);
                if (schemeResult.Success)
                {
                    model = schemeResult.Data;
                    model.SelectedInstitutes = string.Join(",", model.Institutes.Select(s => s.InstituteId));
                }
            }

            await SetComboBoxes(model);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(Scheme model)
        {
            ResponseModel schemeResult;
            if (model.SchemeId == 0)
            {
                schemeResult = await _schemeService.CreateScheme(model);
            }
            else
            {
                schemeResult = await _schemeService.UpdateScheme(model);
            }

            if (schemeResult.Success)
            {
                SetNotification($"Scheme saved successfully!", NotificationTypes.Success, "Scheme");
                return RedirectToAction("Index", "Scheme");
            }

            await SetComboBoxes(model);
            SetNotification(schemeResult.Message, NotificationTypes.Error, "Scheme");
            return View(model);
        }

        public async Task<IActionResult> GetSchemes(
            string? searchValue = null,
            long? instituteId = null
        )
        {
            try
            {
                dynamic filters = new ExpandoObject();
                filters.searchValue = searchValue;
                filters.instituteId = instituteId;

                dynamic grid = GetGridPagination(filters);
                var result = await _schemeService.GetSchemePaged(grid.pagination);
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

        public async Task<IActionResult> AddSchemeInstitute(long schemeId, long instituteId)
        {
            return Ok(await _schemeInstituteService.CreateSchemeInstitute(new SchemeInstitute
            {
                SchemeId = schemeId,
                InstituteId = instituteId,
            }));
        }

        public async Task<IActionResult> RemoveSchemeInstitute(long schemeInstituteId)
        {
            return Ok(await _schemeInstituteService.DeleteSchemeInstitute(schemeInstituteId));
        }

        public async Task<IActionResult> GetSchemeInstitutes(long schemeId)
        {
            try
            {
                dynamic filters = new ExpandoObject();
                filters.schemeId = schemeId;

                var schemeInstituteResult = await _schemeInstituteService.GetSchemeInstitutePaged(
                    new Pagination
                    {
                        Filters = filters
                    });

                var schemeInstitutePaged = schemeInstituteResult.Data as PagedList;
                return PartialView("_InstituteList", schemeInstitutePaged.Data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task SetComboBoxes(Scheme model)
        {
            ViewBag.Institutes = GetSelectList(
                          await _instituteService.GetDropdwon(),
                          "",
                          model.Institutes.Select(s => s.InstituteId).ToList());

            ViewBag.DurationTypes = GetSelectList(
                         await _durationTypeService.GetDropdwon(),
                         "Select Duration Type");
        }

        private bool AuthorizeUser()
        {
            return _userProviderService.UserClaim.IsGts;
        }
    }
}
