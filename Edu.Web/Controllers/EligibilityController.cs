using Core.Repository.Models;
using Core.Utility.Utils;
using Edu.Abstraction.ComplexModels;
using Edu.Abstraction.Enums;
using Edu.Abstraction.Models;
using Edu.Service.Interfaces;
using Edu.Service.Services;
using Edu.Web.Models;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Dynamic;

namespace Edu.Web.Controllers
{
    public class EligibilityController : BaseController
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
        private readonly IFileService _fileService;
        private readonly IUserService _userService;
        private readonly ITemplateService _templateService;

        public EligibilityController(
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
            ITemplateService templateService
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
        }

        public async Task<IActionResult> Index()
        {
            if (!AuthorizeUserGts()) { return AccessDeniedView(); }

            await SetComboBoxes(true);
            return View();
        }

        public async Task<IActionResult> Approver()
        {
            if (!AuthorizeUserApprover()) { return AccessDeniedView(); }

            await SetComboBoxes(false);
            return View();
        }

        public async Task<IActionResult> GetEligibilities(
            string? searchValue = null,
            string? startDate = null,
            string? endDate = null,
            long? schemeId = null,
            long? intakeId = null,
            long? designationId = null,
            long? divisionId = null,
            long? departmentId = null,
            long? locationId = null
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
                filters.divisionId = divisionId;
                filters.departmentId = departmentId;
                filters.locationId = locationId;

                dynamic grid = GetGridPagination(filters);
                var result = await _nominationService.GetEligibilitiesPaged(grid.pagination);
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

        public async Task<ResponseModel> UploadEligibilities(IFormFile file, long schemeId, long intakeId)
        {
            return await ValidateUploadEligibilities(file, schemeId, intakeId);
        }

        public async Task<IActionResult> SaveEligibilities(IFormFile file, long schemeId, long intakeId)
        {
            var response = await ValidateUploadEligibilities(file, schemeId, intakeId);
            if (response.Success)
            {
                return Ok(await _nominationService.CreateNominations((response.Data as List<EligibilityModel>).Cast<Nomination>().ToList()));
            }
            return Ok(response);
        }

        public async Task<IActionResult> PublishEligibilities(long schemeId, long intakeId)
        {
            return Ok(await _nominationService.PublishEligibilities(schemeId, intakeId));
        }

        private async Task SetComboBoxes(bool isGts)
        {
            ViewBag.Schemes = GetSelectList(
                       await _schemeService.GetDropdwon(),
                       "All"
                    );

            ViewBag.Intakes = GetSelectList(
                        new List<DropdownModel>(),
                        "All"
                     );

            ViewBag.Designations = GetSelectList(
                        await _designationService.GetDropdwon(),
                        "DESG: All"
                     );

            ViewBag.Divisions = GetSelectList(
                        await _divisionService.GetDropdwon(),
                        "DIVN: All"
                     );

            ViewBag.Departments = GetSelectList(
                        await _departmentService.GetDropdwon(),
                        "DEPT: All"
                     );

            ViewBag.Locations = GetSelectList(
                        await _locationService.GetDropdwon(),
                        "Location: All"
                     );

            if (isGts)
            {
                ViewBag.Template = (await _templateService.GetTemplateByKey(nameof(TemplateKeys.EligibilityEmail))).TemplateContent;
            }
        }

        private async Task<ResponseModel> ValidateUploadEligibilities(IFormFile file, long schemeId, long intakeId)
        {
            if (file == null || file.Length == 0)
                return new ResponseModel { Success = false, Message = "Please select the file!", StatusCode = StatusCodes.Status400BadRequest };

            var path = await _fileService.CreateFile(file);

            var uploadeligibilities = new List<EligibilityModel>();
            var eligibilityFileError = string.Empty;

            var users = await _userService.GetUserDropdwon();
            var verticals = await _verticalService.GetDropdwon();
            var designations = await _designationService.GetDropdwon();
            var departments = await _departmentService.GetDropdwon();
            var divisions = await _divisionService.GetDropdwon();
            var locations = await _locationService.GetDropdwon();

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = System.IO.File.Open(path, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    int index = 0;
                    int verticalIndex = 0;
                    int staffNoIndex = 1;
                    int staffNameIndex = 2;
                    int dojIndex = 3;
                    int gradeCheckIndex = 4;
                    int relavantExpIndex = 5;
                    int designationIndex = 6;
                    int divisonIndex = 7;
                    int departmentIndex = 8;
                    int locationIndex = 9;
                    int approver1Index = 10;
                    int approver2Index = 11;

                    while (reader.Read()) //Each row of the file
                    {
                        if (index == 0)
                        {
                        }
                        else
                        {

                            var vertical = reader.GetValue(verticalIndex)?.ToString().Trim() ?? "";
                            var staffNo = reader.GetValue(staffNoIndex)?.ToString().Trim() ?? "";
                            var staffName = reader.GetValue(staffNameIndex)?.ToString().Trim() ?? "";
                            var doj = reader.GetValue(dojIndex)?.ToString().Trim() ?? "";
                            var gradeCheck = reader.GetValue(gradeCheckIndex)?.ToString().Trim() ?? "";
                            var relavantExp = reader.GetValue(relavantExpIndex)?.ToString().Trim() ?? "";
                            var designation = reader.GetValue(designationIndex)?.ToString().Trim() ?? "";
                            var divison = reader.GetValue(divisonIndex)?.ToString().Trim() ?? "";
                            var department = reader.GetValue(departmentIndex)?.ToString().Trim() ?? "";
                            var location = reader.GetValue(locationIndex)?.ToString().Trim() ?? "";
                            var approver1 = reader.GetValue(approver1Index)?.ToString().Trim() ?? "";
                            var approver2 = reader.GetValue(approver2Index)?.ToString().Trim() ?? "";

                            if (string.IsNullOrEmpty(vertical) && string.IsNullOrEmpty(staffNo) &&
                                string.IsNullOrEmpty(staffName) && string.IsNullOrEmpty(doj) &&
                                string.IsNullOrEmpty(gradeCheck) && string.IsNullOrEmpty(relavantExp) &&
                                string.IsNullOrEmpty(designation) && string.IsNullOrEmpty(divison) &&
                                string.IsNullOrEmpty(department) && string.IsNullOrEmpty(location) &&
                                string.IsNullOrEmpty(approver1) && string.IsNullOrEmpty(approver2))
                            {
                                continue;
                            }

                            DateTime _doj = DateTime.MinValue;
                            try { _doj = CommonUtils.GetParseDate(doj); } catch { }
                            _ = decimal.TryParse(relavantExp, out decimal _relavantExp);
                            _ = long.TryParse(approver1, out long approver1Id);
                            _ = long.TryParse(approver2, out long approver2Id);
                            _ = long.TryParse(staffNo, out long msilUserId);

                            var _user = users.FirstOrDefault(s => s.MsilUserId == msilUserId);
                            var _vertical = verticals.FirstOrDefault(s => s.Text == vertical);
                            var _designation = designations.FirstOrDefault(s => s.Text == designation);
                            var _divison = divisions.FirstOrDefault(s => s.Text == divison);
                            var _department = departments.FirstOrDefault(s => s.Text == department);
                            var _location = locations.FirstOrDefault(s => s.Text == location);
                            var _approver1 = users.FirstOrDefault(s => s.MsilUserId == approver1Id);
                            var _approver2 = users.FirstOrDefault(s => s.MsilUserId == approver2Id);

                            var rowErrorMessage = string.Empty;

                            if (string.IsNullOrEmpty(vertical))
                            {
                                rowErrorMessage += "<li>Vertical Id has no content</li>";
                            }
                            else if (_vertical == null)
                            {
                                rowErrorMessage += "<li>Vertical did not match</li>";
                            }

                            if (string.IsNullOrEmpty(staffNo))
                            {
                                rowErrorMessage += "<li>Staff No has no content</li>";
                            }
                            else if (_user == null)
                            {
                                rowErrorMessage += "<li>Staff No did not match</li>";
                            }

                            if (_user != null)
                            {
                                if (string.IsNullOrEmpty(staffName))
                                {
                                    rowErrorMessage += "<li>Staff Name has no content</li>";
                                }

                                if (string.IsNullOrEmpty(doj))
                                {
                                    rowErrorMessage += "<li>DOJ has no content</li>";
                                }
                                else
                                {
                                    if (_doj == DateTime.MinValue)
                                    {
                                        rowErrorMessage += "<li>DOJ has not valid date (dd-mm-yyyy)</li>";
                                    }
                                    else if (_user.Doj.HasValue && _doj.Date != _user.Doj.Value)
                                    {
                                        rowErrorMessage += "<li>DOJ did not match</li>";
                                    }
                                }

                                if (string.IsNullOrEmpty(gradeCheck))
                                {
                                    rowErrorMessage += "<li>Grade Check has no content</li>";
                                }

                                if (string.IsNullOrEmpty(relavantExp))
                                {
                                    rowErrorMessage += "<li>Relavant Exp has no content</li>";
                                }

                                if (string.IsNullOrEmpty(designation))
                                {
                                    rowErrorMessage += "<li>Designation has no content</li>";
                                }
                                else if (_designation == null || (_user.DesignationId.HasValue && _user.DesignationId.Value != _designation.Id))
                                {
                                    rowErrorMessage += "<li>Designation did not match</li>";
                                }

                                if (string.IsNullOrEmpty(divison))
                                {
                                    rowErrorMessage += "<li>Divison has no content</li>";
                                }
                                else if (_divison == null || (_user.DivisionId.HasValue && _user.DivisionId.Value != _divison.Id))
                                {
                                    rowErrorMessage += "<li>Divison did not match</li>";
                                }

                                if (string.IsNullOrEmpty(department))
                                {
                                    rowErrorMessage += "<li>Department has no content</li>";
                                }
                                else if (_department == null || (_user.DepartmentId.HasValue && _user.DepartmentId.Value != _department.Id))
                                {
                                    rowErrorMessage += "<li>Department did not match</li>";
                                }

                                if (string.IsNullOrEmpty(location))
                                {
                                    rowErrorMessage += "<li>Location has no content</li>";
                                }
                                else if (_location == null || (_user.LocationId.HasValue && _user.LocationId.Value != _location.Id))
                                {
                                    rowErrorMessage += "<li>Location did not match</li>";
                                }

                                if (string.IsNullOrEmpty(approver1))
                                {
                                    rowErrorMessage += "<li>Approver 1 has no content</li>";
                                }
                                else if (_approver1 == null)
                                {
                                    rowErrorMessage += "<li>Approver 1 did not match</li>";
                                }

                                if (string.IsNullOrEmpty(approver2))
                                {
                                    rowErrorMessage += "<li>Approver 2 has no content</li>";
                                }
                                else if (_approver2 == null)
                                {
                                    rowErrorMessage += "<li>Approver 2 did not match</li>";
                                }
                            }

                            if (!string.IsNullOrEmpty(rowErrorMessage))
                            {
                                rowErrorMessage = $"<ul>Row {index + 1} has error{rowErrorMessage}</ul>";
                                eligibilityFileError += rowErrorMessage;
                                index++;
                                continue;
                            }

                            uploadeligibilities.Add(new EligibilityModel
                            {
                                SchemeId = schemeId,
                                IntakeId = intakeId,
                                UserId = _user.Id,
                                NominationStatusId = NominationStatuses.Nominated.GetHashCode(),
                                Doj = _doj,
                                MsilTenure = (decimal)((DateTime.Now.Date - _doj.Date).TotalDays / 365.25),
                                Grade = gradeCheck,
                                RelevantPrevExp = _relavantExp,
                                StaffName = _user.Text,
                                MsilUserId = _user.MsilUserId,
                                VerticalId = _vertical.Id,
                                Vertical = _vertical.Text,
                                DivisionId = _divison.Id,
                                Division = _divison.Text,
                                DesignationId = _designation.Id,
                                Designation = _designation.Text,
                                DepartmentId = _department.Id,
                                Department = _department.Text,
                                LocationId = _location.Id,
                                Location = _location.Text,
                                ApprovalBy1 = _approver1.Id,
                                Approver1 = $"{_approver1.MsilUserId}-{_approver1.Text}",
                                ApprovalBy2 = _approver2.Id,
                                Approver2 = $"{_approver2.MsilUserId}-{_approver2.Text}",
                            });
                        }
                        index++;
                    }
                }
            }

            _fileService.DeleteFile(path);

            var duplicateUsers = uploadeligibilities.Select(u => u.MsilUserId).GroupBy(x => x)
                                     .Where(g => g.Count() > 1)
                                     .Select(x => x.Key).ToList();


            if (duplicateUsers.Any())
            {
                eligibilityFileError += $"Staff No duplicates in upload file: {string.Join(", ", duplicateUsers)}";
            }

            //var existsUsers = await _nominationService.GetNominationUsers(schemeId, intakeId);
            //var duplicateExistsUsers = uploadeligibilities.Where(u => existsUsers.Contains(u.MsilUserId)).Select(u => u.MsilUserId).ToList();
            //if (duplicateExistsUsers.Any())
            //{
            //    eligibilityFileError += $"Staff No already uploaded: {string.Join(", ", duplicateExistsUsers)}";
            //}

            if (!string.IsNullOrEmpty(eligibilityFileError))
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status400BadRequest, Message = eligibilityFileError };
            }

            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Eligibilities upload successfully",
                Data = uploadeligibilities
            };
        }

        private bool AuthorizeUserGts()
        {
            return _userProviderService.UserClaim.IsGts;
        }

        private bool AuthorizeUserApprover()
        {
            return _userProviderService.UserClaim.IsApprover;
        }
    }
}