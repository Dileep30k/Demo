using AutoQueryable.Core.Models;
using Core.Repository.Models;
using Core.Utility.Utils;
using Edu.Abstraction.ComplexModels;
using Edu.Abstraction.Enums;
using Edu.Abstraction.Models;
using Edu.Service.Interfaces;
using Edu.Web.Models;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Dynamic;
using System.IO;
using System.IO.Compression;

namespace Edu.Web.Controllers
{
    public class ScorecardController : BaseController
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
        private readonly IDocumentService _documentService;

        public ScorecardController(
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
            IDocumentService documentService
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
            _documentService = documentService;
        }

        #region GTS

        public async Task<IActionResult> Index()
        {
            if (!_userProviderService.UserClaim.IsGts) { return AccessDeniedView(); }
            await SetComboBoxes();
            return View();
        }

        public async Task<IActionResult> GetScorecards(
            string? searchValue = null,
            long? schemeId = null,
            long? intakeId = null,
            long? instituteId = null,
            int? isScoreApprove = null
       )
        {
            try
            {
                dynamic filters = new ExpandoObject();
                filters.searchValue = searchValue;
                filters.schemeId = schemeId;
                filters.intakeId = intakeId;
                filters.instituteId = instituteId;
                filters.isScoreApprove = (bool?)(isScoreApprove != null ? isScoreApprove == 1 : null);

                dynamic grid = GetGridPagination(filters);
                var result = await _nominationService.GetScorecardPaged(grid.pagination);
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

        public async Task<IActionResult> UpdateNominationScore(long nominationId, decimal score)
        {
            return Ok(await _nominationService.UpdateNominationScore(new Nomination
            {
                NominationId = nominationId,
                Score = score,
                IsExamTaken = true
            }));
        }

        public async Task<IActionResult> UpdateNominationScoreApproval(long nominationId, bool isScoreApprove)
        {
            return Ok(await _nominationService.UpdateNominationScoreApproval(new Nomination
            {
                NominationId = nominationId,
                IsScoreApprove = isScoreApprove,
            }));
        }

        public async Task<IActionResult> AllowGtsScorecard(long intakeId)
        {
            return Ok(await _nominationService.AllowGtsScorecard(intakeId));
        }

        public async Task<ResponseModel> UploadScorecards(IFormFile scorecardFile, IFormFile documentFile, long schemeId, long intakeId)
        {
            if (scorecardFile == null || scorecardFile.Length == 0)
                return new ResponseModel { Success = false, Message = "Please select the scorecards file!", StatusCode = StatusCodes.Status400BadRequest };

            if (documentFile == null || documentFile.Length == 0)
                return new ResponseModel { Success = false, Message = "Please select the documents file!", StatusCode = StatusCodes.Status400BadRequest };

            var scorecardPath = await _fileService.CreateFile(scorecardFile);
            var documentPath = await _fileService.CreateFile(documentFile);

            var documentFiles = new List<string>();

            var destinationZip = _fileService.GetAbsolutePath($"TempDocs/{intakeId}");
            if (_fileService.DirectoryExists(destinationZip))
            {
                _fileService.DeleteDirectory(destinationZip);
            }
            _fileService.CreateDirectory(destinationZip);
            using (ZipArchive archive = ZipFile.OpenRead(documentPath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    documentFiles.Add(entry.FullName);
                    entry.ExtractToFile(Path.Combine(destinationZip, entry.FullName));
                }
            }

            var uploadeScorecards = new List<ScorecardModel>();
            var scorecardFileError = string.Empty;

            var scorecardUsers = await _userService.GetScorecardUserDropdwon(schemeId, intakeId);

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = System.IO.File.Open(scorecardPath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    int index = 0;
                    int staffNoIndex = 0;
                    int staffNameIndex = 1;
                    int scoreIndex = 2;
                    int fileNameIndex = 3;

                    while (reader.Read()) //Each row of the file
                    {
                        if (index == 0)
                        {
                        }
                        else
                        {
                            var staffNo = reader.GetValue(staffNoIndex)?.ToString().Trim() ?? "";
                            var staffName = reader.GetValue(staffNameIndex)?.ToString().Trim() ?? "";
                            var score = reader.GetValue(scoreIndex)?.ToString().Trim() ?? "";
                            var fileName = reader.GetValue(fileNameIndex)?.ToString().Trim() ?? "";

                            if (string.IsNullOrEmpty(staffNo) &&
                                string.IsNullOrEmpty(staffName) && string.IsNullOrEmpty(score) &&
                                string.IsNullOrEmpty(fileName))
                            {
                                continue;
                            }

                            _ = long.TryParse(staffNo, out long msilUserId);
                            _ = decimal.TryParse(score, out decimal _score);

                            var _user = scorecardUsers.FirstOrDefault(s => s.MsilUserId == msilUserId);

                            var rowErrorMessage = string.Empty;

                            if (string.IsNullOrEmpty(staffNo))
                            {
                                rowErrorMessage += "<li>Staff No has no content</li>";
                            }
                            else if (_user == null)
                            {
                                rowErrorMessage += "<li>Staff No did not match</li>";
                            }

                            if (string.IsNullOrEmpty(staffName))
                            {
                                rowErrorMessage += "<li>Staff Name has no content</li>";
                            }

                            if (string.IsNullOrEmpty(score))
                            {
                                rowErrorMessage += "<li>Score has no content</li>";
                            }

                            if (string.IsNullOrEmpty(fileName))
                            {
                                rowErrorMessage += "<li>Filename has no content</li>";
                            }
                            else if (!documentFiles.Any(s => s == fileName))
                            {
                                rowErrorMessage += "<li>File does not exists in zip</li>";
                            }

                            if (!string.IsNullOrEmpty(rowErrorMessage))
                            {
                                rowErrorMessage = $"<ul>Row {index + 1} has error{rowErrorMessage}</ul>";
                                scorecardFileError += rowErrorMessage;
                                index++;
                                continue;
                            }

                            uploadeScorecards.Add(new ScorecardModel
                            {
                                UserId = _user.Id,
                                MsilUserId = _user.MsilUserId,
                                NominationId = _user.NominationId,
                                Score = _score,
                                FileName = fileName,
                            });
                        }
                        index++;
                    }
                }
            }

            _fileService.DeleteFile(documentPath);
            _fileService.DeleteFile(scorecardPath);

            var duplicateUsers = uploadeScorecards.Select(u => u.MsilUserId).GroupBy(x => x)
                                     .Where(g => g.Count() > 1)
                                     .Select(x => x.Key).ToList();

            if (duplicateUsers.Any())
            {
                scorecardFileError += $"Staff No duplicates: {string.Join(",", duplicateUsers)})";
            }

            if (!string.IsNullOrEmpty(scorecardFileError))
            {
                _fileService.DeleteDirectory(destinationZip);
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status400BadRequest, Message = scorecardFileError };
            }

            var documents = new List<Document>();
            var nominations = new List<Nomination>();
            var provider = new FileExtensionContentTypeProvider();
            string contentType;

            foreach (var uploadeScorecard in uploadeScorecards)
            {
                if (!provider.TryGetContentType(uploadeScorecard.FileName, out contentType))
                {
                    contentType = "application/octet-stream";
                }

                var document = new Document
                {
                    DocumentType = nameof(DocumentTypes.EmployeeScore),
                    FileName = uploadeScorecard.FileName,
                    ContentType = contentType,
                    DocumentTable = "Nominations",
                    DocumentTableId = uploadeScorecard.NominationId,
                };
                _fileService.CreateDocumentFileZipEntry(document, destinationZip);
                documents.Add(document);
                nominations.Add(new Nomination
                {
                    NominationId = uploadeScorecard.NominationId,
                    Score = uploadeScorecard.Score
                });
            }

            _fileService.DeleteDirectory(destinationZip);

            var responseModel = await _documentService.CreateDocuments(documents);
            if (responseModel.Success)
            {
                return await _nominationService.UpdateNominationScores(nominations);
            }

            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status400BadRequest, Message = "Scorecards not uploaded." };
        }

        #endregion

        #region Employee
        public async Task<IActionResult> Employee()
        {
            if (!_userProviderService.UserClaim.IsEmployee) { return AccessDeniedView(); }
            await SetComboBoxes();
            return View();
        }

        public async Task<IActionResult> GetScorecardModel(long intakeId)
        {
            return Ok(await _nominationService.GetScorecardModel(intakeId));
        }

        public async Task<IActionResult> GetScorecardForm(NominationFormModel model)
        {
            return PartialView("_Scorecard", model);
        }

        public async Task<IActionResult> UploadEmpScorecard(IFormFile file, long nominationId, decimal score)
        {
            if (file == null || file.Length == 0)
                return Ok(new ResponseModel { Success = false, Message = "Please select the file!", StatusCode = StatusCodes.Status400BadRequest });

            var result = await _fileService.CreateDocumentFile(new Document
            {
                DocumentType = nameof(DocumentTypes.EmployeeScore),
                DocumentTable = "Nominations",
                DocumentTableId = nominationId,
                DocumentFile = file,
            });

            if (result.Success)
            {
                result = await _documentService.CreateDocument(result.Data);
                if (result.Success)
                {
                    result = await _nominationService.UpdateNominationScore(new Nomination
                    {
                        NominationId = nominationId,
                        Score = score,
                        IsExamTaken = true
                    });
                }
            }

            return Ok(result);
        }

        public async Task<IActionResult> UpdateScorecardExam(long nominationId)
        {
            return Ok(await _nominationService.UpdateNominationScore(new Nomination
            {
                NominationId = nominationId,
                IsExamTaken = false
            }));
        }

        public async Task<IActionResult> RejectNomination(long nominationId)
        {
            return Ok(await _nominationService.UpdateNominationStatus(new Nomination
            {
                NominationId = nominationId,
                NominationStatusId = NominationStatuses.Rejected.GetHashCode()
            })); ;
        }

        #endregion

        public async Task<IActionResult> GetSchemeIntakes(long? schemeId)
        {
            return Ok(await _intakeService.GetDropdwon(schemeId: schemeId));
        }

        public async Task<IActionResult> GetIntakeInstitutes(long intakeId)
        {
            return Ok(await _intakeInstituteService.GetDropdwon(intakeId: intakeId));
        }

        public async Task<IActionResult> DownloadAllScoreCard(long? schemeId, long? intakeId)
        {
            dynamic filters = new ExpandoObject();
            filters.schemeId = schemeId;
            filters.intakeId = intakeId;
            var result = await _nominationService.GetScorecardPaged(new Pagination { Filters = filters });

            var memoryStream = new MemoryStream();
            using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var nomination in result.Data.Data as List<NominationModel>)
                {
                    foreach (var document in nomination.Documents)
                    {
                        zipArchive.CreateEntryFromFile(_fileService.GetAbsolutePath(document.FilePath), $"{nomination.MsilUserId}-{document.FileName}");
                    }
                }
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            return File(memoryStream, "application/zip", "AllScorecard.zip");

        }

        private async Task SetComboBoxes()
        {
            ViewBag.Schemes = GetSelectList(
                       await _schemeService.GetDropdwon(),
                       "All"
                    );

            ViewBag.Intakes = GetSelectList(
                        new List<DropdownModel>(),
                        "All"
                     );

            ViewBag.ScoreAppoves = GetSelectList(
                      new List<DropdownModel> {
                          new DropdownModel { Id = 1, Text = "Approved" },
                          new DropdownModel { Id = 2, Text = "Not Approved" },
                      },
                      "APPROVAL: All"
                   );
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