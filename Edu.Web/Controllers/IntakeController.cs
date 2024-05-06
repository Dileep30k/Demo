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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Edu.Web.Controllers
{
    public class IntakeController : BaseController
    {
        private readonly IIntakeService _intakeService;
        private readonly IIntakeInstituteService _intakeInstituteService;
        private readonly IUserProviderService _userProviderService;
        private readonly ISchemeInstituteService _schemeInstituteService;
        private readonly ISchemeService _schemeService;
        private readonly IDocumentTypeService _documentTypeService;
        private readonly IIntakeDocumentTypeService _intakeDocumentTypeService;
        private readonly ILocationService _locationService;
        private readonly IFileService _fileService;

        public IntakeController(
            IIntakeService intakeService,
            IIntakeInstituteService intakeInstituteService,
            IUserProviderService userProviderService,
            ISchemeInstituteService schemeInstituteService,
            ISchemeService schemeService,
            IDocumentTypeService documentTypeService,
            IIntakeDocumentTypeService intakeDocumentTypeService,
            ILocationService locationService,
            IFileService fileService
        )
        {
            _intakeService = intakeService;
            _intakeInstituteService = intakeInstituteService;
            _userProviderService = userProviderService;
            _schemeInstituteService = schemeInstituteService;
            _schemeService = schemeService;
            _documentTypeService = documentTypeService;
            _intakeDocumentTypeService = intakeDocumentTypeService;
            _locationService = locationService;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            if (!AuthorizeUser()) { return AccessDeniedView(); }
            await SetComboBoxesForList();
            return View();
        }

        public async Task<IActionResult> Manage(long id = 0)
        {
            if (!AuthorizeUser()) { return AccessDeniedView(); }

            var model = new Intake();
            if (id > 0)
            {
                var intakeResult = await _intakeService.GetIntakeById(id);
                if (intakeResult.Success)
                {
                    model = intakeResult.Data;
                    model.SelectedLocations = string.Join(",", model.Locations.Select(s => s.Location));
                    model.SelectedInstitutes = string.Join(",", model.Institutes.Select(s => s.InstituteId));
                    model.SelectedDocumentTypes = string.Join(",", model.DocumentTypes.Select(s => s.DocumentTypeId));
                }
            }

            await SetComboBoxes(model);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(Intake model)
        {
            var scheme = (await _schemeService.GetSchemeById(model.SchemeId)).Data as Scheme;

            ResponseModel intakeResult;
            if (model.IntakeId == 0)
            {
                intakeResult = await _intakeService.CreateIntake(model);
            }
            else
            {
                intakeResult = await _intakeService.UpdateIntake(model);
            }

            if (intakeResult.Success)
            {
                if (model.Brochure != null)
                {
                    var intake = intakeResult.Data as Intake;
                    var result = await _fileService.CreateDocumentFile(new Document
                    {
                        DocumentType = nameof(DocumentTypes.IntakeBrochure),
                        DocumentTable = "Intakes",
                        DocumentTableId = intake.IntakeId,
                        DocumentFile = model.Brochure,
                    });

                    if (result.Success)
                    {
                        var document = result.Data as Document;
                        intake.BrochureFileName = document.FileName;
                        intake.BrochureFilePath = document.FilePath;
                        intakeResult = await _intakeService.UpdateIntakeBrochure(intake);
                    }
                }

                SetNotification($"Intake saved successfully!", NotificationTypes.Success, "Intake");
                return RedirectToAction("Index", "Intake");
            }

            await SetComboBoxes(model);
            SetNotification(intakeResult.Message, NotificationTypes.Error, "Intake");
            return View(model);
        }

        public async Task<IActionResult> GetIntakes(
            string? searchValue = null,
            string? startDate = null,
            string? endDate = null,
            long? schemeId = null,
            long? intakeId = null
        )
        {
            try
            {
                dynamic filters = new ExpandoObject();
                filters.searchValue = searchValue;
                filters.startDate = startDate;
                filters.endDate = endDate;
                filters.schemeId = schemeId;
                filters.intakeId = intakeId;

                dynamic grid = GetGridPagination(filters);
                var result = await _intakeService.GetIntakePaged(grid.pagination);
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

        public async Task<IActionResult> GetSchemeInstitutes(long schemeId)
        {
            return Ok(await _schemeInstituteService.GetDropdwon(schemeId: schemeId));
        }

        public async Task<IActionResult> GetIntakeInstitute(IntakeInstitute institute)
        {
            return PartialView("_IntakeInstitute", institute);
        }

        public async Task<IActionResult> GetSchemeIntakes(long? schemeId)
        {
            return Ok(await _intakeService.GetDropdwon(schemeId: schemeId));
        }

        private async Task SetComboBoxes(Intake model)
        {
            ViewBag.Institutes = GetSelectList(
                          model.SchemeId > 0 ? await _schemeInstituteService.GetDropdwon(schemeId: model.SchemeId) : new List<DropdownModel>(),
                          "",
                          model.Institutes.Select(s => s.InstituteId).ToList());

            ViewBag.DocumentTypes = GetSelectList(
                          await _documentTypeService.GetDropdwon(),
                          "",
                          model.DocumentTypes.Select(s => s.DocumentTypeId).ToList());

            ViewBag.Locations = GetSelectList(
                         await _locationService.GetDropdwon(),
                          "",
                          model.Locations.Select(s => s.LocationId).ToList());

            ViewBag.Schemes = GetSelectList(
                         await _schemeService.GetDropdwon(),
                         "Select Scheme",
                         new List<long> { model.SchemeId });
        }

        private async Task SetComboBoxesForList()
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
