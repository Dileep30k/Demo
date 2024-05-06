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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Net.NetworkInformation;

namespace Edu.Web.Controllers
{
    public class AdmissionController : BaseController
    {
        private readonly IAdmissionService _admissionService;
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
        private readonly IIntakeInstituteService _intakeInstituteService;
        private readonly IAdmissionStatusService _admissionStatusService;
        private readonly IDocumentService _documentService;
        private readonly IBatchService _batchService;

        public AdmissionController(
            IAdmissionService admissionService,
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
            IAdmissionStatusService admissionStatusService,
            IDocumentService documentService,
            IBatchService batchService
        )
        {
            _admissionService = admissionService;
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
            _admissionStatusService = admissionStatusService;
            _documentService = documentService;
            _batchService = batchService;
        }

        public async Task<IActionResult> Index()
        {
            if (!_userProviderService.UserClaim.IsGts) { return AccessDeniedView(); }

            await SetComboBoxes();
            return View();
        }

        public async Task<IActionResult> Approver()
        {
            if (!_userProviderService.UserClaim.IsApprover) { return AccessDeniedView(); }

            await SetComboBoxes();
            return View();
        }

        public async Task<IActionResult> Employee()
        {
            if (!_userProviderService.UserClaim.IsEmployee) { return AccessDeniedView(); }

            await SetComboBoxes();
            return View();
        }

        public async Task<IActionResult> GetAdmission(long schemeId, long intakeId, long instituteId, bool isGts)
        {
            return Ok(await _admissionService.GetAdmission(schemeId, intakeId, instituteId, isGts));
        }

        public async Task<IActionResult> GetAdmissions(
            bool isGts,
            long? schemeId = null,
            long? intakeId = null,
            long? instituteId = null
       )
        {
            try
            {
                dynamic filters = new ExpandoObject();
                filters.schemeId = schemeId;
                filters.intakeId = intakeId;
                filters.instituteId = instituteId;
                filters.isGts = isGts;

                dynamic grid = GetGridPagination(filters);
                var result = await _admissionService.GetAdmissionPaged(grid.pagination);
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

        public async Task<IActionResult> GetAdmissionInstitutes(
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
                var result = await _admissionService.GetAdmissionInstitutes(grid.pagination);
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

        public async Task<IActionResult> GetAdmissionUsers(
            long? schemeId = null,
            long? intakeId = null,
            long? instituteId = null,
            long? admissionStatusId = null,
            long? serviceBondAccepted = null,
            string? searchValue = null,
            string? startDate = null,
            string? endDate = null
       )
        {
            try
            {
                dynamic filters = new ExpandoObject();
                filters.schemeId = schemeId;
                filters.intakeId = intakeId;
                filters.instituteId = instituteId;
                filters.admissionStatusId = admissionStatusId;
                filters.searchValue = searchValue;
                filters.startDate = startDate;
                filters.endDate = endDate;
                if (serviceBondAccepted != null) filters.serviceBondAccepted = serviceBondAccepted == 1;

                dynamic grid = GetGridPagination(filters);
                var result = await _admissionService.GetAdmissionUsers(grid.pagination);
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

        public async Task<IActionResult> GetConfirmAdmissionUsers(
           long? schemeId = null,
           long? intakeId = null,
           long? instituteId = null,
           string? searchValue = null
      )
        {
            try
            {
                dynamic filters = new ExpandoObject();
                filters.schemeId = schemeId;
                filters.intakeId = intakeId;
                filters.instituteId = instituteId;
                filters.searchValue = searchValue;

                dynamic grid = GetGridPagination(filters);
                var result = await _admissionService.GetConfirmAdmissionUsers(grid.pagination);
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

        public async Task<IActionResult> GetIntakeInstitutes(long intakeId)
        {
            return Ok(await _intakeInstituteService.GetDropdwon(intakeId: intakeId));
        }

        public async Task<IActionResult> GetBatches(long schemeId, long intakeId, long instituteId)
        {
            return Ok(await _batchService.GetDropdwon(schemeId: schemeId, intakeId: intakeId, instituteId: instituteId));
        }

        public async Task<IActionResult> GetIntakeDocuments(long intakeId)
        {
            return Ok((await _intakeService.GetIntakeById(intakeId)).Data.DocumentTypes);
        }

        public async Task<ResponseModel> UploadAdmissions(IFormFile file, long schemeId, long intakeId, long instituteId, long admissionId)
        {
            return await ValidateUploadAdmissions(file, schemeId, intakeId, instituteId, admissionId);
        }

        public async Task<IActionResult> SaveAdmissions(IFormFile file, long schemeId, long intakeId, long instituteId, long admissionId)
        {
            var response = await ValidateUploadAdmissions(file, schemeId, intakeId, instituteId, admissionId);
            return Ok(await _admissionService.UploadAdmissions(response.Data));
        }

        public async Task<IActionResult> ApproveAdmissions(Admission admission)
        {
            return Ok(await _admissionService.UpdateAdmission(admission, false));
        }

        public async Task<IActionResult> PublishAdmissions(Admission admission)
        {
            return Ok(await _admissionService.UpdateAdmission(admission, true));
        }

        public async Task<IActionResult> ApproveAdmissionUser(AdmissionUser admissionUser)
        {
            return Ok(await _admissionService.UpdateAdmissionUser(admissionUser));
        }

        public async Task<ResponseModel> UploadConfirmAdmissions(IFormFile file, long admissionId)
        {
            return await ValidateUploadConfirmAdmissions(file, admissionId);
        }

        public async Task<IActionResult> SaveConfirmAdmissions(IFormFile file, long admissionId)
        {
            var response = await ValidateUploadConfirmAdmissions(file, admissionId);
            return Ok(await _admissionService.ConfirmAdmissions(response.Data));
        }

        public async Task<ResponseModel> UpdateAdmissionUserStatus(IFormFile file, long admissionUserId, long admissionStatusId, string remarks)
        {
            var result = await _fileService.CreateDocumentFile(new Document
            {
                DocumentType = nameof(DocumentTypes.AdmissionUser),
                DocumentTable = "AdmissionUsers",
                DocumentTableId = admissionUserId,
                DocumentFile = file,
            });

            if (result.Success)
            {
                result = await _documentService.CreateDocument(result.Data);
                if (result.Success)
                {
                    result = await _admissionService.UpdateAdmissionUserStatus(new AdmissionUser
                    {
                        AdmissionUserId = admissionUserId,
                        AdmissionStatusId = admissionStatusId,
                        ApproverRemarks = remarks
                    });
                }
            }

            return result;
        }

        public async Task<ResponseModel> UpdateAdmissionUserLegal(long admissionUserId, IFormFile? serviceAgreement, IFormFile? suretyBond)
        {
            var result = new ResponseModel();
            if (serviceAgreement != null)
            {
                result = await _fileService.CreateDocumentFile(new Document
                {
                    DocumentType = nameof(DocumentTypes.AdmissionAgreement),
                    DocumentTable = "AdmissionUsers",
                    DocumentTableId = admissionUserId,
                    DocumentFile = serviceAgreement,
                });

                result = await _documentService.CreateDocument(result.Data);
            }
            if (suretyBond != null)
            {

                result = await _fileService.CreateDocumentFile(new Document
                {
                    DocumentType = nameof(DocumentTypes.AdmissionBond),
                    DocumentTable = "AdmissionUsers",
                    DocumentTableId = admissionUserId,
                    DocumentFile = suretyBond,
                });

                result = await _documentService.CreateDocument(result.Data);
            }

            result = await _admissionService.UpdateAdmissionUserLegal(new AdmissionUser
            {
                AdmissionUserId = admissionUserId
            });

            return result;
        }

        private async Task SetComboBoxes()
        {
            ViewBag.Schemes = GetSelectList(
                       await _schemeService.GetDropdwon(),
                       "Select"
                    );

            ViewBag.Intakes = GetSelectList(
                        new List<DropdownModel>(),
                        "N/A"
                     );

            ViewBag.Institutes = GetSelectList(
                     new List<DropdownModel>(),
                     "N/A"
                  );

            ViewBag.Batches = GetSelectList(
                     new List<DropdownModel>(),
                     "N/A"
                  );

            var admissionStatus = await _admissionStatusService.GetDropdwon();

            ViewBag.AdmissionStatuses = GetSelectList(
                      admissionStatus,
                      "Admission Status: All"
                   );

            var updateStatus = new List<long> { AdmissionStatuses.Left.GetHashCode(), AdmissionStatuses.Pause.GetHashCode(), };

            ViewBag.UpdateStatuses = GetSelectList(
                      admissionStatus.Where(s => updateStatus.Contains(s.Id)).ToList(),
                      "Select Status"
                   );

            ViewBag.BondTypes = GetSelectList(
                     new List<DropdownModel> {
                         new DropdownModel { Id = 1, Text = "Yes" },
                         new DropdownModel { Id = 2, Text = "No" },
                     },
                     "Service Bond Acceptance: All"
                  );

            ViewBag.Template = (await _templateService.GetTemplateByKey(nameof(TemplateKeys.AdmissionEmail))).TemplateContent;
        }

        public async Task<ResponseModel> ValidateUploadConfirmAdmissions(IFormFile file, long admissionId)
        {
            if (file == null || file.Length == 0)
                return new ResponseModel { Success = false, Message = "Please select the file!", StatusCode = StatusCodes.Status400BadRequest };

            var path = await _fileService.CreateFile(file);

            var uploadadmissions = new List<AdmissionUserModel>();
            var admissionFileError = string.Empty;

            var nominationUsers = await _userService.GetAdmissionActiveUserDropdwon(admissionId);

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = System.IO.File.Open(path, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    int index = 0;
                    int staffNoIndex = 0;
                    int semesterIndex = 1;

                    while (reader.Read()) //Each row of the file
                    {
                        if (index == 0)
                        {
                        }
                        else
                        {
                            var staffNo = reader.GetValue(staffNoIndex)?.ToString().Trim() ?? "";
                            var semester = reader.GetValue(semesterIndex)?.ToString().Trim() ?? "";

                            if (string.IsNullOrEmpty(staffNo) && string.IsNullOrEmpty(semester))
                            {
                                continue;
                            }

                            _ = long.TryParse(staffNo, out long msilUserId);

                            var _user = nominationUsers.FirstOrDefault(s => s.MsilUserId == msilUserId);

                            var rowErrorMessage = string.Empty;

                            if (string.IsNullOrEmpty(staffNo))
                            {
                                rowErrorMessage += "<li>Staff No has no content</li>";
                            }
                            else if (_user == null)
                            {
                                rowErrorMessage += "<li>Staff No did not match</li>";
                            }

                            if (string.IsNullOrEmpty(semester))
                            {
                                rowErrorMessage += "<li>Semester has no content</li>";
                            }

                            if (!string.IsNullOrEmpty(rowErrorMessage))
                            {
                                rowErrorMessage = $"<ul>Row {index + 1} has error{rowErrorMessage}</ul>";
                                admissionFileError += rowErrorMessage;
                                index++;
                                continue;
                            }

                            uploadadmissions.Add(new AdmissionUserModel
                            {
                                AdmissionId = admissionId,
                                AdmissionUserId = _user.AdmissionUserId,
                                UserId = _user.Id,
                                MsilUserId = _user.MsilUserId,
                                StaffName = _user.Text,
                                MobileNo = _user.MobileNo,
                                Email = _user.Email,
                                Semester = semester
                            });
                        }
                        index++;
                    }
                }
            }

            _fileService.DeleteFile(path);

            var duplicateUsers = uploadadmissions.Select(u => u.MsilUserId).GroupBy(x => x)
                                    .Where(g => g.Count() > 1)
                                    .Select(x => x.Key).ToList();

            if (duplicateUsers.Any())
            {
                admissionFileError += $"Staff No duplicates: {string.Join(",", duplicateUsers)})";
            }

            if (!string.IsNullOrEmpty(admissionFileError))
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status400BadRequest, Message = admissionFileError };
            }

            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Admissions upload successfully",
                Data = uploadadmissions.OrderBy(u => u.Rank).ThenBy(a => a.AdmissionStatusId).ToList()
            };
        }

        public async Task<ResponseModel> ValidateUploadAdmissions(IFormFile file, long schemeId, long intakeId, long instituteId, long admissionId)
        {
            if (file == null || file.Length == 0)
                return new ResponseModel { Success = false, Message = "Please select the file!", StatusCode = StatusCodes.Status400BadRequest };

            var path = await _fileService.CreateFile(file);

            var uploadadmissions = new List<AdmissionUserModel>();
            var admissionFileError = string.Empty;

            var approvers = await _userService.GetUserDropdwon();
            var nominationUsers = await _userService.GetNominationUserDropdwon(schemeId, intakeId, instituteId);

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = System.IO.File.Open(path, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    int index = 0;
                    int rankIndex = 0;
                    int staffNoIndex = 1;
                    int stautsIndex = 2;
                    int approver1Index = 3;
                    int approver2Index = 4;

                    while (reader.Read()) //Each row of the file
                    {
                        if (index == 0)
                        {
                        }
                        else
                        {
                            var rank = reader.GetValue(rankIndex)?.ToString().Trim() ?? "";
                            var staffNo = reader.GetValue(staffNoIndex)?.ToString().Trim() ?? "";
                            var status = reader.GetValue(stautsIndex)?.ToString().Trim() ?? "";
                            var approver1 = reader.GetValue(approver1Index)?.ToString().Trim() ?? "";
                            var approver2 = reader.GetValue(approver2Index)?.ToString().Trim() ?? "";

                            if (string.IsNullOrEmpty(rank) && string.IsNullOrEmpty(staffNo) &&
                                string.IsNullOrEmpty(status) &&
                                string.IsNullOrEmpty(approver1) && string.IsNullOrEmpty(approver2))
                            {
                                continue;
                            }

                            _ = int.TryParse(rank, out int _rank);
                            _ = long.TryParse(approver1, out long approver1Id);
                            _ = long.TryParse(approver2, out long approver2Id);
                            _ = long.TryParse(staffNo, out long msilUserId);
                            _ = Enum.TryParse(status, out AdmissionStatuses _status);

                            var _user = nominationUsers.FirstOrDefault(s => s.MsilUserId == msilUserId);
                            var _approver1 = approvers.FirstOrDefault(s => s.MsilUserId == approver1Id);
                            var _approver2 = approvers.FirstOrDefault(s => s.MsilUserId == approver2Id);

                            var rowErrorMessage = string.Empty;

                            if (string.IsNullOrEmpty(rank))
                            {
                                rowErrorMessage += "<li>Rank has no content</li>";
                            }

                            if (string.IsNullOrEmpty(staffNo))
                            {
                                rowErrorMessage += "<li>Staff No has no content</li>";
                            }
                            else if (_user == null)
                            {
                                rowErrorMessage += "<li>Staff No did not match</li>";
                            }

                            if (string.IsNullOrEmpty(status))
                            {
                                rowErrorMessage += "<li>Status has no content</li>";
                            }
                            else if (_status == 0)
                            {
                                rowErrorMessage += "<li>Status does not match</li>";
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

                            if (!string.IsNullOrEmpty(rowErrorMessage))
                            {
                                rowErrorMessage = $"<ul>Row {index + 1} has error{rowErrorMessage}</ul>";
                                admissionFileError += rowErrorMessage;
                                index++;
                                continue;
                            }

                            uploadadmissions.Add(new AdmissionUserModel
                            {
                                AdmissionUserId = 0,
                                AdmissionId = admissionId,
                                SchemeId = schemeId,
                                IntakeId = intakeId,
                                InstituteId = instituteId,
                                AdmissionStatusId = _status.GetHashCode(),
                                UserId = _user.Id,
                                Rank = _rank,
                                MsilUserId = _user.MsilUserId,
                                StaffName = _user.Text,
                                MobileNo = _user.MobileNo,
                                Email = _user.Email,
                                ApprovalBy1 = _approver1.Id,
                                ApprovalBy2 = _approver2.Id,
                                NominationId = _user.NominationId,
                                NominationInstituteId = _user.NominationInstituteId,
                            });
                        }
                        index++;
                    }
                }
            }
            _fileService.DeleteFile(path);

            var duplicateUsers = uploadadmissions.Select(u => u.MsilUserId).GroupBy(x => x)
                                    .Where(g => g.Count() > 1)
                                    .Select(x => x.Key).ToList();

            if (duplicateUsers.Any())
            {
                admissionFileError += $"Staff No duplicates: {string.Join(",", duplicateUsers)})";
            }

            var confirm = AdmissionStatuses.Confirm.GetHashCode();
            var duplicates = uploadadmissions.Where(a => a.AdmissionStatusId == confirm).Select(u => u.Rank).GroupBy(x => x)
                                       .Where(g => g.Count() > 1)
                                       .Select(x => x.Key).ToList();
            if (duplicates.Any())
            {
                admissionFileError += $"Confirm admissions rank duplicates: {string.Join(",", duplicates)})";
            }

            var waiting = AdmissionStatuses.Waiting.GetHashCode();
            duplicates = uploadadmissions.Where(a => a.AdmissionStatusId == waiting).Select(u => u.Rank).GroupBy(x => x)
                                       .Where(g => g.Count() > 1)
                                       .Select(x => x.Key).ToList();
            if (duplicates.Any())
            {
                admissionFileError += $"Waiting admissions rank duplicates: {string.Join(",", duplicates)})";
            }

            var intake = (await _intakeService.GetIntakeById(intakeId)).Data as Intake;
            var institute = intake.Institutes.FirstOrDefault(i => i.InstituteId == instituteId);
            if (institute.TotalSeats < uploadadmissions.Count(a => a.AdmissionStatusId == confirm))
            {
                admissionFileError += "Admissions are more than Total seat";
            }

            if (!string.IsNullOrEmpty(admissionFileError))
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status400BadRequest, Message = admissionFileError };
            }

            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Admissions upload successfully",
                Data = uploadadmissions.OrderBy(u => u.Rank).ThenBy(a => a.AdmissionStatusId).ToList()
            };
        }
    }
}