using Core.Repository;
using Core.Repository.Models;
using Core.Utility.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edu.Abstraction.ComplexModels;
using Edu.Abstraction.Models;
using Edu.Repository.Contexts;
using Edu.Repository.Interfaces;
using Serilog;
using System.Runtime.ConstrainedExecution;
using Microsoft.EntityFrameworkCore;
using Edu.Abstraction.Enums;

namespace Edu.Repository.Repositories
{
    public class NominationRepository : Repository<Nomination>, INominationRepository
    {
        public NominationRepository(EduDbContext context)
            : base(context)
        { }

        private EduDbContext EduDbContext
        {
            get { return _dbContext as EduDbContext; }
        }

        public async Task<PagedList> GetEligibilitiesPaged(UserClaim userClaim, Pagination pagination)
        {
            string searchValue = IsPropertyExist(pagination.Filters, "searchValue") ? pagination.Filters?.searchValue : null;
            string startDate = IsPropertyExist(pagination.Filters, "startDate") ? pagination.Filters?.startDate : null;
            string endDate = IsPropertyExist(pagination.Filters, "endDate") ? pagination.Filters?.endDate : null;
            long? schemeId = IsPropertyExist(pagination.Filters, "schemeId") ? pagination.Filters?.schemeId : null;
            long? intakeId = IsPropertyExist(pagination.Filters, "intakeId") ? pagination.Filters?.intakeId : null;
            long? designationId = IsPropertyExist(pagination.Filters, "designationId") ? pagination.Filters?.designationId : null;
            long? divisionId = IsPropertyExist(pagination.Filters, "divisionId") ? pagination.Filters?.divisionId : null;
            long? departmentId = IsPropertyExist(pagination.Filters, "departmentId") ? pagination.Filters?.departmentId : null;
            long? locationId = IsPropertyExist(pagination.Filters, "locationId") ? pagination.Filters?.locationId : null;

            DateTime? _startDate = null;
            if (!string.IsNullOrEmpty(startDate))
            {
                _startDate = CommonUtils.GetParseDate(startDate);
            }

            DateTime? _endDate = null;
            if (!string.IsNullOrEmpty(endDate))
            {
                _endDate = CommonUtils.GetParseDate(endDate);
            }

            IRepository<EligibilityModel> repository = new Repository<EligibilityModel>(EduDbContext);
            var query = (from nom in EduDbContext.Nominations
                         join intake in EduDbContext.Intakes on nom.IntakeId equals intake.IntakeId
                         join user in EduDbContext.Users on nom.UserId equals user.UserId
                         join ver in EduDbContext.Verticals on nom.VerticalId equals ver.VerticalId
                         join div in EduDbContext.Divisions on nom.DivisionId equals div.DivisionId
                         join dep in EduDbContext.Departments on nom.DepartmentId equals dep.DepartmentId
                         join des in EduDbContext.Designations on nom.DesignationId equals des.DesignationId
                         join loc in EduDbContext.Locations on nom.LocationId equals loc.LocationId
                         where !nom.IsDeleted
                         && (!schemeId.HasValue || schemeId.Value == nom.SchemeId)
                         && (!intakeId.HasValue || intakeId.Value == nom.IntakeId)
                         && (!designationId.HasValue || designationId.Value == user.DesignationId)
                         && (!divisionId.HasValue || divisionId.Value == user.DivisionId)
                         && (!departmentId.HasValue || departmentId.Value == user.DepartmentId)
                         && (!locationId.HasValue || locationId.Value == nom.LocationId)
                         && (string.IsNullOrEmpty(searchValue) || user.MsilUserId.ToString().Contains(searchValue) ||
                            user.FirstName.Contains(searchValue) || user.LastName.Contains(searchValue))
                         && (!_startDate.HasValue || _startDate.Value.Date <= intake.StartDate.Date)
                         && (!_endDate.HasValue || _endDate.Value.Date >= intake.StartDate.Date)
                         && (userClaim.IsGts || (nom.ApprovalBy1 == userClaim.UserId || nom.ApprovalBy2 == userClaim.UserId))
                         select new EligibilityModel
                         {
                             NominationId = nom.NominationId,
                             MsilUserId = user.MsilUserId,
                             StaffName = user.FirstName + " " + user.LastName,
                             Doj = user.Doj,
                             Grade = nom.Grade,
                             MsilTenure = nom.MsilTenure,
                             Vertical = ver.VerticalName,
                             RelevantPrevExp = nom.RelevantPrevExp,
                             Designation = des.DesignationName,
                             Division = div.DivisionName,
                             Department = dep.DepartmentName,
                             Location = loc.LocationName,
                             IsPublish = nom.IsPublish,
                             IntakeStartDate = intake.StartDate,
                         });

            return await repository.GetPagedReponseAsync(pagination, null, query);
        }

        public async Task<PagedList> GetNominationPaged(UserClaim userClaim, Pagination pagination)
        {
            string searchValue = IsPropertyExist(pagination.Filters, "searchValue") ? pagination.Filters?.searchValue : null;
            string startDate = IsPropertyExist(pagination.Filters, "startDate") ? pagination.Filters?.startDate : null;
            string endDate = IsPropertyExist(pagination.Filters, "endDate") ? pagination.Filters?.endDate : null;
            long? schemeId = IsPropertyExist(pagination.Filters, "schemeId") ? pagination.Filters?.schemeId : null;
            long? intakeId = IsPropertyExist(pagination.Filters, "intakeId") ? pagination.Filters?.intakeId : null;
            long? instituteId = IsPropertyExist(pagination.Filters, "instituteId") ? pagination.Filters?.instituteId : null;
            long? designationId = IsPropertyExist(pagination.Filters, "designationId") ? pagination.Filters?.designationId : null;
            long? departmentId = IsPropertyExist(pagination.Filters, "departmentId") ? pagination.Filters?.departmentId : null;
            long? nominationStatusId = IsPropertyExist(pagination.Filters, "nominationStatusId") ? pagination.Filters?.nominationStatusId : null;

            DateTime? _startDate = null;
            if (!string.IsNullOrEmpty(startDate))
            {
                _startDate = CommonUtils.GetParseDate(startDate);
            }

            DateTime? _endDate = null;
            if (!string.IsNullOrEmpty(endDate))
            {
                _endDate = CommonUtils.GetParseDate(endDate);
            }

            IRepository<NominationModel> repository = new Repository<NominationModel>(EduDbContext);
            var query = (from nom in EduDbContext.Nominations
                         join intake in EduDbContext.Intakes on nom.IntakeId equals intake.IntakeId
                         join user in EduDbContext.Users on nom.UserId equals user.UserId
                         join ver in EduDbContext.Verticals on nom.VerticalId equals ver.VerticalId
                         join div in EduDbContext.Divisions on nom.DivisionId equals div.DivisionId
                         join dep in EduDbContext.Departments on nom.DepartmentId equals dep.DepartmentId
                         join des in EduDbContext.Designations on nom.DesignationId equals des.DesignationId
                         join loc in EduDbContext.Locations on nom.LocationId equals loc.LocationId
                         where !nom.IsDeleted
                         && nom.IsPublish == true
                         && (!schemeId.HasValue || schemeId.Value == nom.SchemeId)
                         && (!intakeId.HasValue || intakeId.Value == nom.IntakeId)
                         && (!designationId.HasValue || designationId.Value == nom.DesignationId)
                         && (!departmentId.HasValue || departmentId.Value == nom.DepartmentId)
                         && (!nominationStatusId.HasValue || nominationStatusId.Value == nom.NominationStatusId)
                         && (!instituteId.HasValue || EduDbContext.NominationInstitutes
                                      .Where(si => si.NominationId == nom.NominationId && instituteId.Value == si.InstituteId)
                                      .Select(i => i.NominationId).Contains(nom.NominationId))
                         && (string.IsNullOrEmpty(searchValue) || user.MsilUserId.ToString().Contains(searchValue) ||
                            user.FirstName.Contains(searchValue) || user.LastName.Contains(searchValue))
                         && (!_startDate.HasValue || _startDate.Value.Date <= intake.StartDate.Date)
                         && (!_endDate.HasValue || _endDate.Value.Date >= intake.StartDate.Date)
                         && (userClaim.IsGts || (nom.ApprovalBy1 == userClaim.UserId || nom.ApprovalBy2 == userClaim.UserId))
                         let approver1 = nom.ApprovalBy1.HasValue ? EduDbContext.Users.FirstOrDefault(u => u.UserId == nom.ApprovalBy1.Value) : null
                         let approver2 = nom.ApprovalBy2.HasValue ? EduDbContext.Users.FirstOrDefault(u => u.UserId == nom.ApprovalBy2.Value) : null
                         select new NominationModel
                         {
                             NominationId = nom.NominationId,
                             MsilUserId = user.MsilUserId,
                             StaffName = user.FirstName + " " + user.LastName,
                             Doj = user.Doj,
                             Grade = nom.Grade,
                             MsilTenure = nom.MsilTenure,
                             Vertical = ver.VerticalName,
                             RelevantPrevExp = nom.RelevantPrevExp,
                             Designation = des.DesignationName,
                             Division = div.DivisionName,
                             Department = dep.DepartmentName,
                             Location = loc.LocationName,
                             IsPublish = nom.IsPublish,
                             NominationStatusId = nom.NominationStatusId,
                             StaffEmail = user.Email,
                             MobileNo = nom.MobileNo,
                             IntakeStartDate = intake.StartDate,
                             Approver1 = approver1 != null ? approver1.FirstName + " " + approver1.LastName : "",
                             Approver1StaffId = approver1 != null ? approver1.MsilUserId : null,
                             Approver2 = approver2 != null ? approver2.FirstName + " " + approver2.LastName : "",
                             Approver2StaffId = approver2 != null ? approver2.MsilUserId : null,
                             Remarks = new List<string> { nom.ApprovalRemarks1, nom.ApprovalRemarks2 },
                             InstituteNames = (from ci in EduDbContext.NominationInstitutes
                                               join ins in EduDbContext.Institutes on ci.InstituteId equals ins.InstituteId
                                               where ci.NominationId == nom.NominationId
                                               select ins.InstituteName).ToList(),
                         });

            return await repository.GetPagedReponseAsync(pagination, null, query);
        }

        public async Task<PagedList> GetNominationApproverPaged(UserClaim userClaim, Pagination pagination)
        {
            string searchValue = IsPropertyExist(pagination.Filters, "searchValue") ? pagination.Filters?.searchValue : null;
            string startDate = IsPropertyExist(pagination.Filters, "startDate") ? pagination.Filters?.startDate : null;
            string endDate = IsPropertyExist(pagination.Filters, "endDate") ? pagination.Filters?.endDate : null;
            long? schemeId = IsPropertyExist(pagination.Filters, "schemeId") ? pagination.Filters?.schemeId : null;
            long? intakeId = IsPropertyExist(pagination.Filters, "intakeId") ? pagination.Filters?.intakeId : null;
            long? instituteId = IsPropertyExist(pagination.Filters, "instituteId") ? pagination.Filters?.instituteId : null;
            long? divisionId = IsPropertyExist(pagination.Filters, "divisionId") ? pagination.Filters?.divisionId : null;
            long? departmentId = IsPropertyExist(pagination.Filters, "departmentId") ? pagination.Filters?.departmentId : null;

            DateTime? _startDate = null;
            if (!string.IsNullOrEmpty(startDate))
            {
                _startDate = CommonUtils.GetParseDate(startDate);
            }

            DateTime? _endDate = null;
            if (!string.IsNullOrEmpty(endDate))
            {
                _endDate = CommonUtils.GetParseDate(endDate);
            }

            var statuses = new List<long>
            {
                NominationStatuses.Accepted.GetHashCode(),
                NominationStatuses.DepApprove.GetHashCode(),
            };

            IRepository<NominationModel> repository = new Repository<NominationModel>(EduDbContext);
            var query = (from nom in EduDbContext.Nominations
                         join scheme in EduDbContext.Schemes on nom.SchemeId equals scheme.SchemeId
                         join user in EduDbContext.Users on nom.UserId equals user.UserId
                         join div in EduDbContext.Divisions on nom.DivisionId equals div.DivisionId
                         join dep in EduDbContext.Departments on nom.DepartmentId equals dep.DepartmentId
                         where !nom.IsDeleted
                         && nom.IsPublish == true
                         && statuses.Contains(nom.NominationStatusId)
                         && (!schemeId.HasValue || schemeId.Value == nom.SchemeId)
                         && (!intakeId.HasValue || intakeId.Value == nom.IntakeId)
                         && (!divisionId.HasValue || divisionId.Value == nom.DivisionId)
                         && (!departmentId.HasValue || departmentId.Value == user.DepartmentId)
                         && (!instituteId.HasValue || EduDbContext.NominationInstitutes
                                      .Where(si => si.NominationId == nom.NominationId && instituteId.Value == si.InstituteId)
                                      .Select(i => i.NominationId).Contains(nom.NominationId))
                         && (string.IsNullOrEmpty(searchValue) || user.MsilUserId.ToString().Contains(searchValue) ||
                            user.FirstName.Contains(searchValue) || user.LastName.Contains(searchValue))
                         && (!_startDate.HasValue || _startDate.Value.Date <= nom.CreatedDate.Date)
                         && (!_endDate.HasValue || _endDate.Value.Date >= nom.CreatedDate.Date)
                         && ((nom.ApprovalBy1 == userClaim.UserId && !nom.ApprovalDate1.HasValue) ||
                             (nom.ApprovalDate1.HasValue && nom.ApprovalBy2 == userClaim.UserId && !nom.ApprovalDate2.HasValue))
                         select new NominationModel
                         {
                             NominationId = nom.NominationId,
                             MsilUserId = user.MsilUserId,
                             StaffName = user.FirstName + " " + user.LastName,
                             Doj = user.Doj,
                             Grade = nom.Grade,
                             MsilTenure = nom.MsilTenure,
                             RelevantPrevExp = nom.RelevantPrevExp,
                             Division = div.DivisionName,
                             Department = dep.DepartmentName,
                             NominationStatusId = nom.NominationStatusId,
                             StaffEmail = user.Email,
                             MobileNo = nom.MobileNo,
                             SchemeName = scheme.SchemeName,
                             InstituteNames = (from ci in EduDbContext.NominationInstitutes
                                               join ins in EduDbContext.Institutes on ci.InstituteId equals ins.InstituteId
                                               where ci.NominationId == nom.NominationId
                                               select ins.InstituteName).ToList(),
                         });

            return await repository.GetPagedReponseAsync(pagination, null, query);
        }

        public async Task<PagedList> GetDropdwon(long? id = null, bool? isActive = null)
        {
            Repository<DropdownModel> repository = new(EduDbContext);
            var query = (from r in EduDbContext.Nominations
                         where !r.IsDeleted &&
                         (!id.HasValue || r.NominationId == id) &&
                         (!isActive.HasValue || r.IsActive == isActive)
                         select new DropdownModel()
                         {
                             Id = r.NominationId,
                             IsActive = r.IsActive
                         });

            return await repository.GetPagedReponseAsync(new Pagination { SortOrderColumn = "Text" }, null, query);
        }

        public async Task<NominationFormModel> GetNominationModel(UserClaim userClaim, long intakeId)
        {
            return await (from nom in EduDbContext.Nominations
                          join user in EduDbContext.Users on nom.UserId equals user.UserId
                          join div in EduDbContext.Divisions on nom.DivisionId equals div.DivisionId
                          join dep in EduDbContext.Departments on nom.DepartmentId equals dep.DepartmentId
                          join intake in EduDbContext.Intakes on nom.IntakeId equals intake.IntakeId
                          join scheme in EduDbContext.Schemes on intake.SchemeId equals scheme.SchemeId
                          join noms in EduDbContext.NominationStatuses on nom.NominationStatusId equals noms.NominationStatusId
                          where nom.UserId == userClaim.UserId
                          && nom.IntakeId == intakeId
                          select new NominationFormModel
                          {
                              IntakeId = nom.IntakeId,
                              NominationId = nom.NominationId,
                              MobileNo = nom.MobileNo,
                              Gender = nom.Gender,
                              NominationStatusId = nom.NominationStatusId,
                              NominationStatusName = noms.NominationStatusName,
                              MsilUserId = user.MsilUserId,
                              StaffName = user.FirstName + " " + user.LastName,
                              Division = div.DivisionName,
                              Department = dep.DepartmentName,
                              ExamDate = intake.ExamDate,
                              NominationCutoffDate = intake.NominationCutoffDate,
                              SchemeName = scheme.SchemeName,
                              SelectedInstitutes = string.Join(",", from ni in EduDbContext.NominationInstitutes
                                                                    where nom.NominationId == ni.NominationId
                                                                    select ni.InstituteId),
                          }).FirstOrDefaultAsync();
        }

        public async Task<PagedList> GetScorecardPaged(Pagination pagination)
        {
            string searchValue = IsPropertyExist(pagination.Filters, "searchValue") ? pagination.Filters?.searchValue : null;
            long? schemeId = IsPropertyExist(pagination.Filters, "schemeId") ? pagination.Filters?.schemeId : null;
            long? intakeId = IsPropertyExist(pagination.Filters, "intakeId") ? pagination.Filters?.intakeId : null;
            long? instituteId = IsPropertyExist(pagination.Filters, "instituteId") ? pagination.Filters?.instituteId : null;
            bool? isScoreApprove = IsPropertyExist(pagination.Filters, "isScoreApprove") ? pagination.Filters?.isScoreApprove : null;
            var statuses = new List<long> { NominationStatuses.DivApprove.GetHashCode() };
            var docuementType = nameof(DocumentTypes.EmployeeScore);
            var docuementTable = "Nominations";
            var today = CommonUtils.GetDefaultDateTime().Date;

            IRepository<NominationModel> repository = new Repository<NominationModel>(EduDbContext);
            var query = (from nom in EduDbContext.Nominations
                         join intake in EduDbContext.Intakes on nom.IntakeId equals intake.IntakeId
                         join user in EduDbContext.Users on nom.UserId equals user.UserId
                         join ver in EduDbContext.Verticals on nom.VerticalId equals ver.VerticalId
                         where !nom.IsDeleted
                         && (intake.IsGTSScoreUpload == true || nom.IsExamTaken == true)
                         && statuses.Contains(nom.NominationStatusId)
                         && (!isScoreApprove.HasValue || isScoreApprove.Value == nom.IsScoreApprove)
                         && (!schemeId.HasValue || schemeId.Value == nom.SchemeId)
                         && (!intakeId.HasValue || intakeId.Value == nom.IntakeId)
                         && (!instituteId.HasValue || EduDbContext.NominationInstitutes
                                      .Where(si => si.NominationId == nom.NominationId && instituteId.Value == si.InstituteId)
                                      .Select(i => i.NominationId).Contains(nom.NominationId))
                         && (string.IsNullOrEmpty(searchValue) || user.MsilUserId.ToString().Contains(searchValue) ||
                            user.FirstName.Contains(searchValue) || user.LastName.Contains(searchValue))
                         select new NominationModel
                         {
                             NominationId = nom.NominationId,
                             NominationStatusId = nom.NominationStatusId,
                             Vertical = ver.VerticalName,
                             MsilUserId = user.MsilUserId,
                             StaffName = user.FirstName + " " + user.LastName,
                             StaffEmail = user.Email,
                             ExamDate = intake.ExamDate,
                             Score = nom.Score,
                             MobileNo = nom.MobileNo,
                             IsScoreApprove = nom.IsScoreApprove,
                             IsExamTaken = nom.IsExamTaken,
                             IsEdit = intake.ExamDate.Date <= today && intake.ScorecardCutoffDate.Date >= today,
                             Documents = (from doc in EduDbContext.Documents
                                          where doc.DocumentTableId == nom.NominationId && doc.DocumentTable == docuementTable && doc.DocumentType == docuementType
                                          select new DocumentListModel { FileName = doc.FileName, FilePath = doc.FilePath }).ToList(),
                             InstituteNames = (from ci in EduDbContext.NominationInstitutes
                                               join ins in EduDbContext.Institutes on ci.InstituteId equals ins.InstituteId
                                               where ci.NominationId == nom.NominationId
                                               select ins.InstituteName).ToList(),
                         });

            return await repository.GetPagedReponseAsync(pagination, null, query);
        }

        public async Task<NominationFormModel> GetScorecardModel(UserClaim userClaim, long intakeId)
        {
            var statuses = new List<long> { NominationStatuses.DivApprove.GetHashCode() };
            var today = CommonUtils.GetDefaultDateTime().Date;

            return await (from nom in EduDbContext.Nominations
                          join user in EduDbContext.Users on nom.UserId equals user.UserId
                          join intake in EduDbContext.Intakes on nom.IntakeId equals intake.IntakeId
                          where nom.UserId == userClaim.UserId
                          && nom.IntakeId == intakeId
                          && statuses.Contains(nom.NominationStatusId)
                          && intake.IsGTSScoreUpload == false
                          && intake.ExamDate.Date <= today
                          && intake.ScorecardCutoffDate.Date >= today
                          select new NominationFormModel
                          {
                              IntakeId = nom.IntakeId,
                              NominationId = nom.NominationId,
                              NominationStatusId = nom.NominationStatusId,
                              MobileNo = nom.MobileNo,
                              MsilUserId = user.MsilUserId,
                              StaffName = user.FirstName + " " + user.LastName,
                              ExamDate = intake.ExamDate,
                              IsExamTaken = nom.IsExamTaken,
                              Score = nom.Score,
                          }).FirstOrDefaultAsync();
        }

        public async Task<bool> AllowGtsScorecard(UserClaim userClaim, long intakeId)
        {
            var statuses = new List<long> { NominationStatuses.DivApprove.GetHashCode() };
            var today = CommonUtils.GetDefaultDateTime().Date;

            return await (from nom in EduDbContext.Nominations
                          join intake in EduDbContext.Intakes on nom.IntakeId equals intake.IntakeId
                          where nom.IntakeId == intakeId
                          && statuses.Contains(nom.NominationStatusId)
                          && intake.IsGTSScoreUpload == true
                          && intake.ExamDate.Date <= today
                          && intake.ScorecardCutoffDate.Date >= today
                          select nom.IntakeId).AnyAsync();
        }

        public async Task<int> GetTotalEligible(dynamic filters)
        {
            string startDate = IsPropertyExist(filters, "startDate") ? filters?.startDate : null;
            string endDate = IsPropertyExist(filters, "endDate") ? filters?.endDate : null;
            long? schemeId = IsPropertyExist(filters, "schemeId") ? filters?.schemeId : null;
            long? intakeId = IsPropertyExist(filters, "intakeId") ? filters?.intakeId : null;

            DateTime? _startDate = null;
            if (!string.IsNullOrEmpty(startDate))
            {
                _startDate = CommonUtils.GetParseDate(startDate);
            }

            DateTime? _endDate = null;
            if (!string.IsNullOrEmpty(endDate))
            {
                _endDate = CommonUtils.GetParseDate(endDate);
            }

            return await (from nom in EduDbContext.Nominations
                          where !nom.IsDeleted
                          && (!schemeId.HasValue || schemeId.Value == nom.SchemeId)
                          && (!intakeId.HasValue || intakeId.Value == nom.IntakeId)
                          && nom.IsPublish == true
                          select nom.NominationId).CountAsync();
        }

        public async Task<int> GetTotalNomination(dynamic filters)
        {
            string startDate = IsPropertyExist(filters, "startDate") ? filters?.startDate : null;
            string endDate = IsPropertyExist(filters, "endDate") ? filters?.endDate : null;
            long? schemeId = IsPropertyExist(filters, "schemeId") ? filters?.schemeId : null;
            long? intakeId = IsPropertyExist(filters, "intakeId") ? filters?.intakeId : null;

            DateTime? _startDate = null;
            if (!string.IsNullOrEmpty(startDate))
            {
                _startDate = CommonUtils.GetParseDate(startDate);
            }

            DateTime? _endDate = null;
            if (!string.IsNullOrEmpty(endDate))
            {
                _endDate = CommonUtils.GetParseDate(endDate);
            }

            return await (from nom in EduDbContext.Nominations
                          where !nom.IsDeleted
                          && (!schemeId.HasValue || schemeId.Value == nom.SchemeId)
                          && (!intakeId.HasValue || intakeId.Value == nom.IntakeId)
                          && nom.IsPublish == true
                          && nom.ApprovalDate1.HasValue
                          && nom.ApprovalDate2.HasValue
                          select nom.NominationId).CountAsync();
        }

        public async Task<int> GetTotalInstitute(dynamic filters)
        {
            string startDate = IsPropertyExist(filters, "startDate") ? filters?.startDate : null;
            string endDate = IsPropertyExist(filters, "endDate") ? filters?.endDate : null;
            long? schemeId = IsPropertyExist(filters, "schemeId") ? filters?.schemeId : null;
            long? intakeId = IsPropertyExist(filters, "intakeId") ? filters?.intakeId : null;

            DateTime? _startDate = null;
            if (!string.IsNullOrEmpty(startDate))
            {
                _startDate = CommonUtils.GetParseDate(startDate);
            }

            DateTime? _endDate = null;
            if (!string.IsNullOrEmpty(endDate))
            {
                _endDate = CommonUtils.GetParseDate(endDate);
            }

            return await (from ii in EduDbContext.IntakeInstitutes
                          join intake in EduDbContext.Intakes on ii.IntakeId equals intake.IntakeId
                          where !ii.IsDeleted
                          && (!schemeId.HasValue || schemeId.Value == intake.SchemeId)
                          && (!intakeId.HasValue || intakeId.Value == ii.IntakeId)
                          select ii.IntakeInstituteId).CountAsync();
        }

        public async Task<int> GetPendingNominationAcceptance(dynamic filters)
        {
            string startDate = IsPropertyExist(filters, "startDate") ? filters?.startDate : null;
            string endDate = IsPropertyExist(filters, "endDate") ? filters?.endDate : null;
            long? schemeId = IsPropertyExist(filters, "schemeId") ? filters?.schemeId : null;
            long? intakeId = IsPropertyExist(filters, "intakeId") ? filters?.intakeId : null;

            DateTime? _startDate = null;
            if (!string.IsNullOrEmpty(startDate))
            {
                _startDate = CommonUtils.GetParseDate(startDate);
            }

            DateTime? _endDate = null;
            if (!string.IsNullOrEmpty(endDate))
            {
                _endDate = CommonUtils.GetParseDate(endDate);
            }

            var submitted = NominationStatuses.Submitted.GetHashCode();

            return await (from nom in EduDbContext.Nominations
                          where !nom.IsDeleted
                          && (!schemeId.HasValue || schemeId.Value == nom.SchemeId)
                          && (!intakeId.HasValue || intakeId.Value == nom.IntakeId)
                          && nom.IsPublish == true
                          && nom.NominationStatusId == submitted
                          select nom.NominationId).CountAsync();
        }

        public async Task<List<NominationUserDropdownModel>> GetAllPendingScorecardUser(List<long> intakeIds)
        {
            var statuses = new List<long> { NominationStatuses.DivApprove.GetHashCode() };
            return await (from nom in EduDbContext.Nominations
                          join intake in EduDbContext.Intakes on nom.IntakeId equals intake.IntakeId
                          join user in EduDbContext.Users on nom.UserId equals user.UserId
                          where !nom.IsDeleted
                          && statuses.Contains(nom.NominationStatusId)
                          && intakeIds.Contains(nom.IntakeId)
                          && intake.IsGTSScoreUpload == false
                          && nom.IsPublish == true
                          && !nom.IsExamTaken.HasValue
                          select new NominationUserDropdownModel()
                          {
                              Id = nom.UserId,
                              Text = user.FirstName + " " + user.LastName,
                              MsilUserId = user.MsilUserId,
                              Email = user.Email,
                              MobileNo = nom.MobileNo,
                              SchemeId = nom.SchemeId,
                              IntakeId = nom.IntakeId,
                          }).ToListAsync();

        }

        public async Task<Nomination> GetEmployeeNomination(long schemeId, long intakeId, long userId)
        {
            return await (from nom in EduDbContext.Nominations
                          where userId == nom.UserId &&
                          (schemeId == nom.SchemeId) &&
                          (intakeId == nom.IntakeId)
                          select nom).FirstOrDefaultAsync();
        }

        public async Task<List<long>> GetNominationUsers(long schemeId, long intakeId)
        {
            return await (from nom in EduDbContext.Nominations
                          join user in EduDbContext.Users on nom.UserId equals user.UserId
                          where schemeId == nom.SchemeId
                          && intakeId == nom.IntakeId
                          select user.MsilUserId).ToListAsync();
        }

        public async Task<List<NominationApproverEmailModel>> GetNominationApprover1()
        {
            var accepted = NominationStatuses.Accepted.GetHashCode();
            var today = DateTime.Today;

            return await (from nom in EduDbContext.Nominations
                          join user in EduDbContext.Users on nom.UserId equals user.UserId
                          join intake in EduDbContext.Intakes on nom.IntakeId equals intake.IntakeId
                          join scheme in EduDbContext.Schemes on intake.SchemeId equals scheme.SchemeId
                          where !nom.IsDeleted
                          && nom.IsPublish == true
                          && nom.NominationStatusId == accepted
                          && !nom.ApprovalDate1.HasValue
                          && intake.NominationCutoffDate >= today
                          orderby intake.IntakeId, intake.NominationCutoffDate
                          let app1 = EduDbContext.Users.FirstOrDefault(u => u.UserId == nom.ApprovalBy1)
                          select new NominationApproverEmailModel
                          {
                              IntakeId = nom.IntakeId,
                              MsilUserId = user.MsilUserId,
                              StaffName = user.FirstName + " " + user.LastName,
                              StaffEmail = user.Email,
                              MobileNo = nom.MobileNo,
                              SchemeName = scheme.SchemeName,
                              IntakeName = intake.IntakeName,
                              Approver1Name = app1.FirstName + " " + app1.LastName + " - " + app1.MsilUserId,
                              ApproverEmail = app1.Email,
                          })
                          .ToListAsync();
        }

        public async Task<List<NominationApproverEmailModel>> GetNominationApprover2(List<long> ids)
        {
            return await (from nom in EduDbContext.Nominations
                          join user in EduDbContext.Users on nom.UserId equals user.UserId
                          join intake in EduDbContext.Intakes on nom.IntakeId equals intake.IntakeId
                          join scheme in EduDbContext.Schemes on intake.SchemeId equals scheme.SchemeId
                          where ids.Contains(nom.NominationId)
                          orderby intake.IntakeId, intake.NominationCutoffDate
                          let app1 = EduDbContext.Users.FirstOrDefault(u => u.UserId == nom.ApprovalBy1)
                          let app2 = EduDbContext.Users.FirstOrDefault(u => u.UserId == nom.ApprovalBy2)
                          select new NominationApproverEmailModel
                          {
                              IntakeId = nom.IntakeId,
                              MsilUserId = user.MsilUserId,
                              StaffName = user.FirstName + " " + user.LastName,
                              StaffEmail = user.Email,
                              MobileNo = nom.MobileNo,
                              SchemeName = scheme.SchemeName,
                              IntakeName = intake.IntakeName,
                              Approver1Name = app1.FirstName + " " + app1.LastName + " - " + app1.MsilUserId,
                              Approver2Name = app2.FirstName + " " + app2.LastName + " - " + app2.MsilUserId,
                              ApproverEmail = app2.Email,
                          })
                          .ToListAsync();
        }
    }
}
