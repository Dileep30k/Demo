using Core.Repository.Constants;
using Core.Repository.Models;
using Edu.Abstraction.ComplexModels;
using Edu.Abstraction.Enums;
using Edu.Abstraction.Models;
using Edu.Service.Interfaces;
using Edu.Service.Services;
using Edu.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Dynamic;

namespace Edu.Web.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly INominationService _nominationService;
        private readonly IAdmissionService _admissionService;
        private readonly ISchemeService _schemeService;
        private readonly IIntakeService _intakeService;
        private readonly IUserProviderService _userProviderService;
        private readonly IIntakeInstituteService _intakeInstituteService;

        public DashboardController(
            INominationService nominationService,
            IAdmissionService admissionService,
            ISchemeService schemeService,
            IIntakeService intakeService,
            IUserProviderService userProviderService,
            IIntakeInstituteService intakeInstituteService
        )
        {
            _nominationService = nominationService;
            _admissionService = admissionService;
            _schemeService = schemeService;
            _intakeService = intakeService;
            _intakeInstituteService = intakeInstituteService;
            _userProviderService = userProviderService;
        }

        public async Task<IActionResult> Index()
        {
            if (!AuthorizeUser()) { return AccessDeniedView(); }
            await GtsComboBoxes();
            return View();
        }

        public async Task<IActionResult> Employee()
        {
            if (!AuthorizeUser()) { return AccessDeniedView(); }
            await EmpComboBoxes();
            return View();
        }

        public async Task<IActionResult> AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> GetSchemeIntakes(long? schemeId)
        {
            if (_userProviderService.UserClaim.IsGts)
            {
                return Ok(await _intakeService.GetDropdwon(schemeId: schemeId));
            }
            if (_userProviderService.UserClaim.IsEmployee)
            {
                if (schemeId.HasValue)
                {
                    return Ok(await _intakeService.GetEmployeeDropdwon(schemeId.Value, _userProviderService.UserClaim.UserId));
                }
            }
            return Ok(new List<DropdownModel>());
        }
        public async Task<IActionResult> GetIntakeInstitutes(long intakeId)
        {
            if (_userProviderService.UserClaim.IsEmployee)
            {
                return Ok(await _intakeInstituteService.GetDropdwon(intakeId: intakeId));
            }
            return Ok(new List<DropdownModel>());
        }

        public async Task<IActionResult> GetTotalEligible(string? startDate = null, string? endDate = null, long? schemeId = null, long? intakeId = null)
        {
            try
            {
                dynamic filters = new ExpandoObject();
                filters.startDate = startDate;
                filters.endDate = endDate;
                filters.schemeId = schemeId;
                filters.intakeId = intakeId;

                var result = await _nominationService.GetTotalEligible(filters);
                return Ok(result);
            }
            catch (Exception) { return BadRequest(); }
        }

        public async Task<IActionResult> GetTotalNomination(string? startDate = null, string? endDate = null, long? schemeId = null, long? intakeId = null)
        {
            try
            {
                dynamic filters = new ExpandoObject();
                filters.startDate = startDate;
                filters.endDate = endDate;
                filters.schemeId = schemeId;
                filters.intakeId = intakeId;

                var result = await _nominationService.GetTotalNomination(filters);
                return Ok(result);
            }
            catch (Exception) { return BadRequest(); }
        }

        public async Task<IActionResult> GetTotalInstitute(string? startDate = null, string? endDate = null, long? schemeId = null, long? intakeId = null)
        {
            try
            {
                dynamic filters = new ExpandoObject();
                filters.startDate = startDate;
                filters.endDate = endDate;
                filters.schemeId = schemeId;
                filters.intakeId = intakeId;

                var result = await _nominationService.GetTotalInstitute(filters);
                return Ok(result);
            }
            catch (Exception) { return BadRequest(); }
        }

        public async Task<IActionResult> GetTotalAdmission(string? startDate = null, string? endDate = null, long? schemeId = null, long? intakeId = null)
        {
            try
            {
                dynamic filters = new ExpandoObject();
                filters.startDate = startDate;
                filters.endDate = endDate;
                filters.schemeId = schemeId;
                filters.intakeId = intakeId;

                var result = await _admissionService.GetTotalAdmission(filters);
                return Ok(result);
            }
            catch (Exception) { return BadRequest(); }
        }

        public async Task<IActionResult> GetTotalWaillist(string? startDate = null, string? endDate = null, long? schemeId = null, long? intakeId = null)
        {
            try
            {
                dynamic filters = new ExpandoObject();
                filters.startDate = startDate;
                filters.endDate = endDate;
                filters.schemeId = schemeId;
                filters.intakeId = intakeId;

                var result = await _admissionService.GetTotalWaillist(filters);
                return Ok(result);
            }
            catch (Exception) { return BadRequest(); }
        }

        public async Task<IActionResult> GetPendingServiceAgreement(string? startDate = null, string? endDate = null, long? schemeId = null, long? intakeId = null)
        {
            try
            {
                dynamic filters = new ExpandoObject();
                filters.startDate = startDate;
                filters.endDate = endDate;
                filters.schemeId = schemeId;
                filters.intakeId = intakeId;

                var result = await _admissionService.GetPendingServiceAgreement(filters);
                return Ok(result);
            }
            catch (Exception) { return BadRequest(); }
        }

        public async Task<IActionResult> GetPendingNominationAcceptance(string? startDate = null, string? endDate = null, long? schemeId = null, long? intakeId = null)
        {
            try
            {
                dynamic filters = new ExpandoObject();
                filters.startDate = startDate;
                filters.endDate = endDate;
                filters.schemeId = schemeId;
                filters.intakeId = intakeId;

                var result = await _nominationService.GetPendingNominationAcceptance(filters);
                return Ok(result);
            }
            catch (Exception) { return BadRequest(); }
        }

        public async Task<IActionResult> GetPendingAdmissionConfirmation(string? startDate = null, string? endDate = null, long? schemeId = null, long? intakeId = null)
        {
            try
            {
                dynamic filters = new ExpandoObject();
                filters.startDate = startDate;
                filters.endDate = endDate;
                filters.schemeId = schemeId;
                filters.intakeId = intakeId;

                var result = await _admissionService.GetPendingAdmissionConfirmation(filters);
                return Ok(result);
            }
            catch (Exception) { return BadRequest(); }
        }

        public async Task<IActionResult> GetAdmissionPagedByDesg(string? startDate = null, string? endDate = null, long? schemeId = null, long? intakeId = null)
        {
            try
            {
                dynamic filters = new ExpandoObject();
                filters.startDate = startDate;
                filters.endDate = endDate;
                filters.schemeId = schemeId;
                filters.intakeId = intakeId;

                var result = await _admissionService.GetAdmissionPagedByDesg(new Pagination { Filters = filters });
                return Ok(result.Data.Data);
            }
            catch (Exception) { return BadRequest(); }
        }

        public async Task<IActionResult> GetAdmissionPagedByDiv(string? startDate = null, string? endDate = null, long? schemeId = null, long? intakeId = null)
        {
            try
            {
                dynamic filters = new ExpandoObject();
                filters.startDate = startDate;
                filters.endDate = endDate;
                filters.schemeId = schemeId;
                filters.intakeId = intakeId;

                var result = await _admissionService.GetAdmissionPagedByDiv(new Pagination { Filters = filters });
                return Ok(result.Data.Data);
            }
            catch (Exception) { return BadRequest(); }
        }

        public async Task<IActionResult> GetEmployeeNomination(long schemeId, long intakeId)
        {
            return Ok(await _nominationService.GetEmployeeNomination(schemeId, intakeId, _userProviderService.UserClaim.UserId));
        }

        private async Task GtsComboBoxes()
        {
            ViewBag.Schemes = GetSelectList(
                      await _schemeService.GetDropdwon(),
                      "All"
                   );

            ViewBag.Intakes = GetSelectList(
                        new List<DropdownModel>(),
                        "All"
                     );
        }

        private async Task EmpComboBoxes()
        {
            var schemes = await _schemeService.GetEmployeeDropdwon(_userProviderService.UserClaim.UserId);
            ViewBag.Schemes = GetSelectList(
                     schemes,
                     schemes.Count > 0 ? "Select" : "N/A"
                  );

            ViewBag.Intakes = GetSelectList(
                       schemes.Count == 1 ?
                       await _intakeService.GetEmployeeDropdwon(schemes.FirstOrDefault().Id, _userProviderService.UserClaim.UserId) :
                       new List<DropdownModel>(),
                        "N/A"
                     );
        }

        private bool AuthorizeUser()
        {
            return _userProviderService.IsAuthenticated();
        }
    }
}