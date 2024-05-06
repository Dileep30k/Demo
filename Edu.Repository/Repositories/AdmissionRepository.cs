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
using Microsoft.OpenApi.Writers;
using Edu.Abstraction.Enums;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace Edu.Repository.Repositories
{
    public class AdmissionRepository : Repository<Admission>, IAdmissionRepository
    {
        public AdmissionRepository(EduDbContext context)
            : base(context)
        { }

        private EduDbContext EduDbContext
        {
            get { return _dbContext as EduDbContext; }
        }

        public async Task<Admission> GetAdmission(UserClaim userClaim, long schemeId, long intakeId, long instituteId, bool isGts)
        {
            var admission = await (from adm in EduDbContext.Admissions
                                   where !adm.IsDeleted &&
                                   (schemeId == adm.SchemeId) &&
                                   (intakeId == adm.IntakeId) &&
                                   (instituteId == adm.InstituteId) &&
                                   (isGts || (adm.ApprovalBy1 == userClaim.UserId || adm.ApprovalBy2 == userClaim.UserId))
                                   select adm).FirstOrDefaultAsync();

            if (userClaim.IsGts)
            {
                return admission;
            }

            if (admission != null && userClaim.IsApprover &&
              ((admission.ApprovalBy1 != userClaim.UserId && admission.ApprovalBy2 != userClaim.UserId) ||
               (admission.ApprovalBy2 == userClaim.UserId && admission.Approved1 != true)))
            {
                return null;
            }

            return admission;
        }

        public async Task<PagedList> GetAdmissionPaged(UserClaim userClaim, Pagination pagination)
        {
            long? schemeId = IsPropertyExist(pagination.Filters, "schemeId") ? pagination.Filters?.schemeId : null;
            long? intakeId = IsPropertyExist(pagination.Filters, "intakeId") ? pagination.Filters?.intakeId : null;
            long? instituteId = IsPropertyExist(pagination.Filters, "instituteId") ? pagination.Filters?.instituteId : null;
            bool isGts = IsPropertyExist(pagination.Filters, "isGts") ? pagination.Filters?.isGts : false;

            IRepository<AdmissionUserModel> repository = new Repository<AdmissionUserModel>(EduDbContext);
            var query = (from au in EduDbContext.AdmissionUsers
                         join a in EduDbContext.Admissions on au.AdmissionId equals a.AdmissionId
                         join nom in EduDbContext.Nominations on au.NominationId equals nom.NominationId
                         join user in EduDbContext.Users on nom.UserId equals user.UserId
                         join admssion in EduDbContext.Admissions on au.AdmissionId equals admssion.AdmissionId
                         orderby au.Rank, au.AdmissionStatusId
                         where !a.IsDeleted
                         && (!schemeId.HasValue || schemeId.Value == admssion.SchemeId)
                         && (!intakeId.HasValue || intakeId.Value == admssion.IntakeId)
                         && (!instituteId.HasValue || instituteId.Value == admssion.InstituteId)
                         && (isGts || (a.ApprovalBy1 == userClaim.UserId || a.ApprovalBy2 == userClaim.UserId))

                         select new AdmissionUserModel
                         {
                             AdmissionUserId = au.AdmissionUserId,
                             AdmissionStatusId = au.AdmissionStatusId,
                             Rank = au.Rank,
                             MsilUserId = user.MsilUserId,
                             UserId = user.UserId,
                             Email = user.Email,
                             MobileNo = nom.MobileNo,
                             StaffName = user.FirstName + " " + user.LastName,
                         });

            return await repository.GetPagedReponseAsync(pagination, null, query);
        }

        public async Task<PagedList> GetDropdwon(long? id = null, bool? isActive = null)
        {
            Repository<DropdownModel> repository = new(EduDbContext);
            var query = (from r in EduDbContext.Admissions
                         where !r.IsDeleted &&
                         (!id.HasValue || r.AdmissionId == id) &&
                         (!isActive.HasValue || r.IsActive == isActive)
                         select new DropdownModel()
                         {
                             Id = r.AdmissionId,
                             IsActive = r.IsActive
                         });

            return await repository.GetPagedReponseAsync(new Pagination { SortOrderColumn = "Text" }, null, query);
        }

        public async Task<PagedList> GetAdmissionInstitutes(UserClaim userClaim, Pagination pagination)
        {
            long? schemeId = IsPropertyExist(pagination.Filters, "schemeId") ? pagination.Filters?.schemeId : null;
            long? intakeId = IsPropertyExist(pagination.Filters, "intakeId") ? pagination.Filters?.intakeId : null;

            IRepository<AdmissionInstituteModel> repository = new Repository<AdmissionInstituteModel>(EduDbContext);
            var query = (from nomi in EduDbContext.NominationInstitutes
                         join institute in EduDbContext.Institutes on nomi.InstituteId equals institute.InstituteId
                         join nom in EduDbContext.Nominations on nomi.NominationId equals nom.NominationId
                         join au in EduDbContext.AdmissionUsers on nomi.NominationInstituteId equals au.NominationInstituteId into nomi_au
                         from au in nomi_au.DefaultIfEmpty()
                         join admssion in EduDbContext.Admissions on au.AdmissionId equals admssion.AdmissionId into au_admssion
                         from admssion in au_admssion.DefaultIfEmpty()
                         let intakeIntitute = EduDbContext.IntakeInstitutes.FirstOrDefault(ii => ii.IntakeId == nom.IntakeId && ii.InstituteId == nomi.InstituteId)
                         where (admssion == null || admssion.IsPublish == true)
                         && schemeId.Value == nom.SchemeId
                         && intakeId.Value == nom.IntakeId
                         && userClaim.UserId == nom.UserId
                         select new AdmissionInstituteModel
                         {
                             AdmissionUserId = au != null ? au.AdmissionUserId : null,
                             AdmissionStatusId = au != null ? au.AdmissionStatusId : null,
                             Rank = au != null ? au.Rank : null,
                             IsConfirmByEmp = au != null ? au.IsConfirmByEmp : null,
                             IsBondAccepted = au != null ? au.IsBondAccepted : null,
                             BondAcceptedDate = au != null ? au.BondAcceptedDate : null,
                             InstituteName = institute.InstituteName,
                             AdmissionCutoffDate = intakeIntitute != null ? intakeIntitute.AdmissionCutoffDate : null,
                         });

            return await repository.GetPagedReponseAsync(pagination, null, query);
        }

        public async Task<PagedList> GetAdmissionUsers(Pagination pagination)
        {
            string searchValue = IsPropertyExist(pagination.Filters, "searchValue") ? pagination.Filters?.searchValue : null;
            string startDate = IsPropertyExist(pagination.Filters, "startDate") ? pagination.Filters?.startDate : null;
            string endDate = IsPropertyExist(pagination.Filters, "endDate") ? pagination.Filters?.endDate : null;
            long? schemeId = IsPropertyExist(pagination.Filters, "schemeId") ? pagination.Filters?.schemeId : null;
            long? intakeId = IsPropertyExist(pagination.Filters, "intakeId") ? pagination.Filters?.intakeId : null;
            long? instituteId = IsPropertyExist(pagination.Filters, "instituteId") ? pagination.Filters?.instituteId : null;
            long? admissionStatusId = IsPropertyExist(pagination.Filters, "admissionStatusId") ? pagination.Filters?.admissionStatusId : null;
            bool? serviceBondAccepted = IsPropertyExist(pagination.Filters, "serviceBondAccepted") ? pagination.Filters?.serviceBondAccepted : null;

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

            var docuementTypes = new List<string> { nameof(DocumentTypes.AdmissionAgreement), nameof(DocumentTypes.AdmissionBond) };
            var docuementTable = "AdmissionUsers";

            IRepository<AdmissionUserModel> repository = new Repository<AdmissionUserModel>(EduDbContext);
            var query = (from au in EduDbContext.AdmissionUsers
                         join admssion in EduDbContext.Admissions on au.AdmissionId equals admssion.AdmissionId
                         join nomi in EduDbContext.NominationInstitutes on au.NominationInstituteId equals nomi.NominationInstituteId
                         join institute in EduDbContext.Institutes on nomi.InstituteId equals institute.InstituteId
                         join nom in EduDbContext.Nominations on au.NominationId equals nom.NominationId
                         join intake in EduDbContext.Intakes on nom.IntakeId equals intake.IntakeId
                         join user in EduDbContext.Users on nom.UserId equals user.UserId
                         join status in EduDbContext.AdmissionStatuses on au.AdmissionStatusId equals status.AdmissionStatusId
                         orderby au.Rank, au.AdmissionStatusId
                         where !admssion.IsDeleted
                         && admssion.IsPublish == true
                         && schemeId.Value == admssion.SchemeId
                         && intakeId.Value == admssion.IntakeId
                         && instituteId.Value == admssion.InstituteId
                         && (!serviceBondAccepted.HasValue || serviceBondAccepted.Value == au.IsBondAccepted)
                         && (!admissionStatusId.HasValue || admissionStatusId.Value == au.AdmissionStatusId)
                         && (string.IsNullOrEmpty(searchValue) || user.MsilUserId.ToString().Contains(searchValue) ||
                            user.FirstName.Contains(searchValue) || user.LastName.Contains(searchValue))
                        && (!_startDate.HasValue || _startDate.Value.Date <= intake.StartDate.Date)
                         && (!_endDate.HasValue || _endDate.Value.Date >= intake.StartDate.Date)
                         select new AdmissionUserModel
                         {
                             AdmissionId = au.AdmissionId,
                             AdmissionUserId = au.AdmissionUserId,
                             AdmissionStatusId = au.AdmissionStatusId,
                             Status = status.AdmissionStatusName,
                             Rank = au.Rank,
                             IsConfirmByEmp = au.IsConfirmByEmp,
                             EmployeeRemarks = au.EmployeeRemarks,
                             IsConfirmByInstitute = au.IsConfirmByInstitute,
                             ApproverRemarks = au.ApproverRemarks,
                             MsilUserId = user.MsilUserId,
                             UserId = user.UserId,
                             Email = user.Email,
                             MobileNo = nom.MobileNo,
                             StaffName = user.FirstName + " " + user.LastName,
                             IntakeStartDate = intake.StartDate,
                             Documents = (from doc in EduDbContext.Documents
                                          where doc.DocumentTableId == au.AdmissionUserId && doc.DocumentTable == docuementTable && docuementTypes.Contains(doc.DocumentType)
                                          select new DocumentListModel { FileName = doc.FileName, FilePath = doc.FilePath }).ToList(),
                         });

            return await repository.GetPagedReponseAsync(pagination, null, query);
        }

        public async Task<PagedList> GetConfirmAdmissionUsers(Pagination pagination)
        {
            string searchValue = IsPropertyExist(pagination.Filters, "searchValue") ? pagination.Filters?.searchValue : null;
            long? schemeId = IsPropertyExist(pagination.Filters, "schemeId") ? pagination.Filters?.schemeId : null;
            long? intakeId = IsPropertyExist(pagination.Filters, "intakeId") ? pagination.Filters?.intakeId : null;
            long? instituteId = IsPropertyExist(pagination.Filters, "instituteId") ? pagination.Filters?.instituteId : null;

            var docuementType = nameof(DocumentTypes.AdmissionUser);
            var docuementTable = "AdmissionUsers";

            IRepository<ConfirmAdmissionUserModel> repository = new Repository<ConfirmAdmissionUserModel>(EduDbContext);
            var query = (from au in EduDbContext.AdmissionUsers
                         join admssion in EduDbContext.Admissions on au.AdmissionId equals admssion.AdmissionId
                         join nomi in EduDbContext.NominationInstitutes on au.NominationInstituteId equals nomi.NominationInstituteId
                         join institute in EduDbContext.Institutes on nomi.InstituteId equals institute.InstituteId
                         join nom in EduDbContext.Nominations on au.NominationId equals nom.NominationId
                         join user in EduDbContext.Users on nom.UserId equals user.UserId
                         join status in EduDbContext.AdmissionStatuses on au.AdmissionStatusId equals status.AdmissionStatusId
                         join ver in EduDbContext.Verticals on nom.VerticalId equals ver.VerticalId
                         join div in EduDbContext.Divisions on nom.DivisionId equals div.DivisionId
                         join dep in EduDbContext.Departments on nom.DepartmentId equals dep.DepartmentId
                         join des in EduDbContext.Designations on nom.DesignationId equals des.DesignationId
                         join loc in EduDbContext.Locations on nom.LocationId equals loc.LocationId
                         where !admssion.IsDeleted
                         && au.IsConfirmByInstitute == true
                         && admssion.IsPublish == true
                         && admssion.IsConfirmUpload == true
                         && schemeId.Value == admssion.SchemeId
                         && intakeId.Value == admssion.IntakeId
                         && instituteId.Value == admssion.InstituteId
                         && (string.IsNullOrEmpty(searchValue) || user.MsilUserId.ToString().Contains(searchValue) ||
                            user.FirstName.Contains(searchValue) || user.LastName.Contains(searchValue))
                         let reportingManager = EduDbContext.Users.FirstOrDefault(u => u.MsilUserId == user.ManagerId.Value)
                         select new ConfirmAdmissionUserModel
                         {
                             AdmissionUserId = au.AdmissionUserId,
                             AdmissionStatusId = au.AdmissionStatusId,
                             MsilUserId = user.MsilUserId,
                             StaffName = user.FirstName + " " + user.LastName,
                             Status = status.AdmissionStatusName,
                             Semester = au.Semester,
                             Vertical = ver.VerticalName,
                             Designation = des.DesignationName,
                             Division = div.DivisionName,
                             Department = dep.DepartmentName,
                             Location = loc.LocationName,
                             Rank = au.Rank,
                             ReportingManager = reportingManager != null ? reportingManager.FirstName + " " + reportingManager.LastName : "",
                             Remarks = au.ApproverRemarks,
                             Email = user.Email,
                             MobileNo = nom.MobileNo,
                             Documents = (from doc in EduDbContext.Documents
                                          where doc.DocumentTableId == au.AdmissionUserId && doc.DocumentTable == docuementTable && doc.DocumentType == docuementType
                                          select new DocumentListModel { FileName = doc.FileName, FilePath = doc.FilePath }).ToList(),

                         });

            return await repository.GetPagedReponseAsync(pagination, null, query);
        }

        public async Task<int> GetTotalAdmission(dynamic filters)
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

            var confimred = AdmissionStatuses.Active.GetHashCode();
            return await (from au in EduDbContext.AdmissionUsers
                          join admssion in EduDbContext.Admissions on au.AdmissionId equals admssion.AdmissionId
                          where !admssion.IsDeleted
                          && (!schemeId.HasValue || schemeId.Value == admssion.SchemeId)
                          && (!intakeId.HasValue || intakeId.Value == admssion.IntakeId)
                          && au.AdmissionStatusId == confimred
                          select au.AdmissionId).CountAsync();
        }

        public async Task<int> GetTotalWaillist(dynamic filters)
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
            var waitlist = AdmissionStatuses.Waiting.GetHashCode();
            return await (from au in EduDbContext.AdmissionUsers
                          join admssion in EduDbContext.Admissions on au.AdmissionId equals admssion.AdmissionId
                          where !admssion.IsDeleted
                          && (!schemeId.HasValue || schemeId.Value == admssion.SchemeId)
                          && (!intakeId.HasValue || intakeId.Value == admssion.IntakeId)
                          && au.AdmissionStatusId == waitlist
                          select au.AdmissionId).CountAsync();
        }

        public async Task<int> GetPendingServiceAgreement(dynamic filters)
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

            return await (from au in EduDbContext.AdmissionUsers
                          join admssion in EduDbContext.Admissions on au.AdmissionId equals admssion.AdmissionId
                          where !admssion.IsDeleted
                          && (!schemeId.HasValue || schemeId.Value == admssion.SchemeId)
                          && (!intakeId.HasValue || intakeId.Value == admssion.IntakeId)
                          && au.IsConfirmByEmp == true
                          && !au.IsBondAccepted.HasValue
                          select au.AdmissionId).CountAsync();
        }

        public async Task<int> GetPendingAdmissionConfirmation(dynamic filters)
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

            var confimred = AdmissionStatuses.Confirm.GetHashCode();
            return await (from au in EduDbContext.AdmissionUsers
                          join admssion in EduDbContext.Admissions on au.AdmissionId equals admssion.AdmissionId
                          where !admssion.IsDeleted
                          && (!schemeId.HasValue || schemeId.Value == admssion.SchemeId)
                          && (!intakeId.HasValue || intakeId.Value == admssion.IntakeId)
                          && au.AdmissionStatusId == confimred
                          && !au.IsConfirmByEmp.HasValue
                          select au.AdmissionId).CountAsync();
        }

        public async Task<PagedList> GetAdmissionPagedByDesg(Pagination pagination)
        {
            string startDate = IsPropertyExist(pagination.Filters, "startDate") ? pagination.Filters?.startDate : null;
            string endDate = IsPropertyExist(pagination.Filters, "endDate") ? pagination.Filters?.endDate : null;
            long? schemeId = IsPropertyExist(pagination.Filters, "schemeId") ? pagination.Filters?.schemeId : null;
            long? intakeId = IsPropertyExist(pagination.Filters, "intakeId") ? pagination.Filters?.intakeId : null;

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

            var active = AdmissionStatuses.Active.GetHashCode();
            IRepository<ChartDataModel> repository = new Repository<ChartDataModel>(EduDbContext);
            var query = (from au in EduDbContext.AdmissionUsers
                         join admssion in EduDbContext.Admissions on au.AdmissionId equals admssion.AdmissionId
                         join nom in EduDbContext.Nominations on au.NominationId equals nom.NominationId
                         join desg in EduDbContext.Designations on nom.DesignationId equals desg.DesignationId
                         where !admssion.IsDeleted
                         && (!schemeId.HasValue || schemeId.Value == admssion.SchemeId)
                         && (!intakeId.HasValue || intakeId.Value == admssion.IntakeId)
                         && au.AdmissionStatusId == active
                         group au by desg.DesignationName into au_group
                         select new ChartDataModel
                         {
                             Count = au_group.Count(),
                             Name = au_group.Key,
                         });

            return await repository.GetPagedReponseAsync(pagination, null, query);
        }

        public async Task<PagedList> GetAdmissionPagedByDiv(Pagination pagination)
        {
            string startDate = IsPropertyExist(pagination.Filters, "startDate") ? pagination.Filters?.startDate : null;
            string endDate = IsPropertyExist(pagination.Filters, "endDate") ? pagination.Filters?.endDate : null;
            long? schemeId = IsPropertyExist(pagination.Filters, "schemeId") ? pagination.Filters?.schemeId : null;
            long? intakeId = IsPropertyExist(pagination.Filters, "intakeId") ? pagination.Filters?.intakeId : null;

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

            var active = AdmissionStatuses.Active.GetHashCode();
            IRepository<ChartDataModel> repository = new Repository<ChartDataModel>(EduDbContext);
            var query = (from au in EduDbContext.AdmissionUsers
                         join admssion in EduDbContext.Admissions on au.AdmissionId equals admssion.AdmissionId
                         join nom in EduDbContext.Nominations on au.NominationId equals nom.NominationId
                         join div in EduDbContext.Divisions on nom.DivisionId equals div.DivisionId
                         where !admssion.IsDeleted
                         && (!schemeId.HasValue || schemeId.Value == admssion.SchemeId)
                         && (!intakeId.HasValue || intakeId.Value == admssion.IntakeId)
                         && au.AdmissionStatusId == active
                         group au by div.DivisionName into au_group
                         select new ChartDataModel
                         {
                             Count = au_group.Count(),
                             Name = au_group.Key,
                         });

            return await repository.GetPagedReponseAsync(pagination, null, query);
        }
    }
}
