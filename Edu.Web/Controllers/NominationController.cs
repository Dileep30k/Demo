using AutoQueryable.Core.Models;
using Core.Repository.Models;
using Edu.Abstraction.ComplexModels;
using Edu.Abstraction.Enums;
using Edu.Abstraction.Models;
using Edu.Service.Interfaces;
using Edu.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Dynamic;

namespace Edu.Web.Controllers
{
    public class NominationController : BaseController
    {
        private readonly INominationService _nominationService;
        private readonly ISchemeService _schemeService;
        private readonly IDivisionService _divisionService;
        private readonly IDepartmentService _departmentService;
        private readonly IDesignationService _designationService;
        private readonly ILocationService _locationService;
        private readonly IVerticalService _verticalService;
        private readonly IUserProviderService _userProviderService;
        private readonly IIntakeService _intakeService;
        private readonly IIntakeInstituteService _intakeInstituteService;
        private readonly IFileService _fileService;
        private readonly IUserService _userService;
        private readonly ITemplateService _templateService;
        private readonly INominationStatusService _nominationStatusService;

        public NominationController(
            INominationService nominationService,
            ISchemeService schemeService,
            IDivisionService divisionService,
            IDepartmentService departmentService,
            IDesignationService designationService,
            ILocationService locationService,
            IVerticalService verticalService,
            IUserProviderService userProviderService,
            IIntakeService intakeService,
            IFileService fileService,
            IUserService userService,
            ITemplateService templateService,
            IIntakeInstituteService intakeInstituteService,
            INominationStatusService nominationStatusService
        )
        {
            _nominationService = nominationService;
            _schemeService = schemeService;
            _divisionService = divisionService;
            _designationService = designationService;
            _locationService = locationService;
            _verticalService = verticalService;
            _departmentService = departmentService;
            _userProviderService = userProviderService;
            _intakeService = intakeService;
            _fileService = fileService;
            _userService = userService;
            _templateService = templateService;
            _intakeInstituteService = intakeInstituteService;
            _nominationStatusService = nominationStatusService;
        }

        #region GTS

        public async Task<IActionResult> Index()
        {
            if (!_userProviderService.UserClaim.IsGts) { return AccessDeniedView(); }
            await SetComboBoxes(true, false, false);
            return View();
        }

        public async Task<IActionResult> GetNominations(
            string? searchValue = null,
            string? startDate = null,
            string? endDate = null,
            long? schemeId = null,
            long? intakeId = null,
            long? instituteId = null,
            long? designationId = null,
            long? departmentId = null,
            long? nominationStatusId = null
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
                filters.designationId = designationId;
                filters.instituteId = instituteId;
                filters.departmentId = departmentId;
                filters.nominationStatusId = nominationStatusId;

                dynamic grid = GetGridPagination(filters);
                var result = await _nominationService.GetNominationPaged(grid.pagination);
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

        #endregion

        #region Approver
        public async Task<IActionResult> Approver()
        {
            if (!_userProviderService.UserClaim.IsApprover) { return AccessDeniedView(); }
            await SetComboBoxes(false, true, false);
            return View();
        }

        public async Task<IActionResult> GetNominationApprover(
            string? searchValue = null,
            string? startDate = null,
            string? endDate = null,
            long? schemeId = null,
            long? intakeId = null,
            long? instituteId = null,
            long? divisionId = null,
            long? departmentId = null
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
                filters.divisionId = divisionId;
                filters.instituteId = instituteId;
                filters.departmentId = departmentId;

                dynamic grid = GetGridPagination(filters);
                var result = await _nominationService.GetNominationApproverPaged(grid.pagination);
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

        public async Task<IActionResult> UpdateNomination(List<long> ids, int type, string remarks)
        {
            return Ok(await _nominationService.UpdateNomination(ids, type, remarks));
        }

        #endregion

        #region Employee
        public async Task<IActionResult> Employee()
        {
            if (!_userProviderService.UserClaim.IsEmployee) { return AccessDeniedView(); }
            await SetComboBoxes(false, false, true);
            return View();
        }

        public async Task<IActionResult> GetSchemeIntakes(long? schemeId)
        {
            return Ok(await _intakeService.GetDropdwon(schemeId: schemeId));
        }

        public async Task<IActionResult> GetNominationModel(long intakeId)
        {
            return Ok(await _nominationService.GetNominationModel(intakeId));
        }

        public async Task<IActionResult> GetNominationInstitutes(long intakeId)
        {
            dynamic filters = new ExpandoObject();
            filters.intakeId = intakeId;
            var result = await _intakeInstituteService.GetIntakeInstitutePaged(new Pagination
            {
                Filters = filters
            });
            var paged = result.Data as PagedList;
            return PartialView("_EmployeeInstitute", paged.Data);
        }

        public async Task<IActionResult> GetNominationForm(NominationFormModel model)
        {
            var selected = !string.IsNullOrEmpty(model.SelectedInstitutes) ?
                model.SelectedInstitutes.Split(',').Select(long.Parse).ToList() : null;
            ViewBag.Institutes = GetSelectList(
                await _intakeInstituteService.GetDropdwon(intakeId: model.IntakeId),
                "",
                selected);
            return PartialView("_Nomination", model);
        }

        public async Task<IActionResult> AcceptNomination(NominationFormModel nomination)
        {
            nomination.NominationStatusId = NominationStatuses.Accepted.GetHashCode();
            return Ok(await _nominationService.UpdateNomination(nomination));
        }

        public async Task<IActionResult> RejectNomination(NominationFormModel nomination)
        {
            nomination.NominationStatusId = NominationStatuses.Rejected.GetHashCode();
            return Ok(await _nominationService.UpdateNomination(nomination));
        }

        #endregion

        public async Task<IActionResult> GetIntakeInstitutes(long intakeId)
        {
            return Ok(await _intakeInstituteService.GetDropdwon(intakeId: intakeId));
        }

        private async Task SetComboBoxes(bool isGTS, bool isApprover, bool isEmployee)
        {
            ViewBag.Schemes = GetSelectList(
                       await _schemeService.GetDropdwon(),
                       "All"
                    );

            ViewBag.Intakes = GetSelectList(
                        new List<DropdownModel>(),
                        "All"
                     );

            var departments = await _departmentService.GetDropdwon();
            ViewBag.Departments = GetSelectList(
            departments,
            "DEPT: All"
            );

            if (isGTS)
            {
                ViewBag.Institutes = GetSelectList(
                      new List<DropdownModel>(),
                      "All"
                   );

                ViewBag.Designations = GetSelectList(
                        await _designationService.GetDropdwon(),
                        "DESG: All"
                     );

                ViewBag.Statuses = GetSelectList(
                        await _nominationStatusService.GetDropdwon(),
                        "STATUS: All"
                     );
            }

            if (isApprover)
            {
                ViewBag.Divisions = GetSelectList(
                await _divisionService.GetDropdwon(),
                "DIVN: All"
                );

                if (_userProviderService.UserClaim.DepartmentId.HasValue)
                {
                    departments = departments.Where(d => d.Id == _userProviderService.UserClaim.DepartmentId.Value).ToList();
                }
                else
                {
                    departments = new List<DropdownModel>();
                }

                ViewBag.Departments = GetSelectList(
                departments
                );
            }

            if (isEmployee)
            {
                ViewBag.Template = (await _templateService.GetTemplateByKey(nameof(TemplateKeys.NominationForm))).TemplateContent;
            }

        }
        private bool AuthorizeUser()
        {
            return _userProviderService.UserClaim.IsGts
                || _userProviderService.UserClaim.IsApprover
                || _userProviderService.UserClaim.IsEmployee
                ;
        }
    }
}