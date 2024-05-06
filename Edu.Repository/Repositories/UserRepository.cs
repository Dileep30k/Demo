using Core.Repository;
using Core.Repository.Models;
using Core.Utility.Utils;
using Edu.Abstraction.ComplexModels;
using Edu.Abstraction.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edu.Repository.Contexts;
using Edu.Repository.Interfaces;
using Edu.Abstraction.Enums;

namespace Edu.Repository.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(EduDbContext context)
            : base(context)
        { }

        private EduDbContext EduDbContext
        {
            get { return _dbContext as EduDbContext; }
        }

        public async Task<PagedList> GetUserPaged(Pagination pagination)
        {
            string searchText = IsPropertyExist(pagination.Filters, "searchText") ? pagination.Filters?.searchText : null;

            IRepository<UserModel> repositoryUserModel = new Repository<UserModel>(EduDbContext);
            var query = (from u in EduDbContext.Users
                         where !u.IsDeleted &&
                         !u.IsAdmin && // Skip System Admin to List Anywhere
                         (string.IsNullOrEmpty(searchText) || (u.Email.Contains(searchText) ||
                         u.FirstName.Contains(searchText) || u.LastName.Contains(searchText) ||
                         u.MsilUserId.ToString().Contains(searchText) || u.MobileNo.Contains(searchText)
                         ))
                         select new UserModel()
                         {
                             UserId = u.UserId,
                             MsilUserId = u.MsilUserId,
                             FirstName = u.FirstName,
                             LastName = u.LastName,
                             Email = u.Email,
                             MobileNo = u.MobileNo,
                         });

            return await repositoryUserModel.GetPagedReponseAsync(pagination, null, query);
        }

        public async Task<PagedList> GetDropdwon(long? id = null, bool? isActive = null)
        {
            Repository<DropdownModel> repositoryDropdownModel = new(EduDbContext);
            var query = (from u in EduDbContext.Users
                         where !u.IsDeleted &&
                         !u.IsAdmin && // Skip System Admin to List Anywhere
                         (!id.HasValue || u.UserId == id) &&
                         (!isActive.HasValue || u.IsActive == isActive)
                         select new DropdownModel()
                         {
                             Id = u.UserId,
                             Text = u.FirstName + " " + u.LastName,
                             IsActive = u.IsActive
                         });

            return await repositoryDropdownModel.GetPagedReponseAsync(new Pagination { SortOrderColumn = "Text" }, null, query);
        }

        public async Task<PagedList> GetUserDropdwon(long? id = null, bool? isActive = null)
        {
            Repository<UserDropdownModel> repositoryDropdownModel = new(EduDbContext);
            var query = (from u in EduDbContext.Users
                         where !u.IsDeleted &&
                         !u.IsAdmin && // Skip System Admin to List Anywhere
                         (!id.HasValue || u.UserId == id) &&
                         (!isActive.HasValue || u.IsActive == isActive)
                         select new UserDropdownModel()
                         {
                             Id = u.UserId,
                             Text = u.FirstName + " " + u.LastName,
                             IsActive = u.IsActive,
                             ManagerId = u.ManagerId,
                             MsilUserId = u.MsilUserId,
                             Doj = u.Doj,
                             VerticalId = u.VerticalId,
                             DivisionId = u.DivisionId,
                             DesignationId = u.DesignationId,
                             DepartmentId = u.DepartmentId,
                             LocationId = u.LocationId,
                         });

            return await repositoryDropdownModel.GetPagedReponseAsync(new Pagination { SortOrderColumn = "Text" }, null, query);
        }

        public async Task<PagedList> GetNominationUserDropdwon(long schemeId, long intakeId, long instituteId)
        {
            Repository<NominationUserDropdownModel> repositoryDropdownModel = new(EduDbContext);
            var query = (from nom in EduDbContext.Nominations
                         join user in EduDbContext.Users on nom.UserId equals user.UserId
                         where !nom.IsDeleted
                         && nom.SchemeId == schemeId
                         && nom.IntakeId == intakeId
                         && nom.IsPublish == true
                         && nom.IsExamTaken == true
                         && nom.IsScoreApprove == true
                         && EduDbContext.NominationInstitutes
                                      .Where(si => si.NominationId == nom.NominationId && instituteId == si.InstituteId)
                                      .Select(i => i.NominationId).Contains(nom.NominationId)
                         select new NominationUserDropdownModel()
                         {
                             Id = nom.UserId,
                             Text = user.FirstName + " " + user.LastName,
                             MsilUserId = user.MsilUserId,
                             Email = user.Email,
                             MobileNo = nom.MobileNo,
                             IsActive = nom.IsActive,
                             NominationId = nom.NominationId,
                             NominationInstituteId = EduDbContext.NominationInstitutes.FirstOrDefault(ni => ni.NominationId == nom.NominationId && ni.InstituteId == instituteId).NominationInstituteId
                         });

            return await repositoryDropdownModel.GetPagedReponseAsync(new Pagination { SortOrderColumn = "Text" }, null, query);
        }

        public async Task<PagedList> GetScorecardUserDropdwon(long schemeId, long intakeId)
        {
            var statuses = new List<long> { NominationStatuses.DivApprove.GetHashCode() };
            Repository<NominationUserDropdownModel> repositoryDropdownModel = new(EduDbContext);
            var query = (from nom in EduDbContext.Nominations
                         join intake in EduDbContext.Intakes on nom.IntakeId equals intake.IntakeId
                         join user in EduDbContext.Users on nom.UserId equals user.UserId
                         where !nom.IsDeleted
                         && intake.IsGTSScoreUpload == true
                         && !nom.IsExamTaken.HasValue
                         && statuses.Contains(nom.NominationStatusId)
                         && schemeId == nom.SchemeId
                         && intakeId == nom.IntakeId
                         select new NominationUserDropdownModel()
                         {
                             Id = nom.UserId,
                             Text = user.FirstName + " " + user.LastName,
                             MsilUserId = user.MsilUserId,
                             Email = user.Email,
                             MobileNo = nom.MobileNo,
                             IsActive = nom.IsActive,
                             NominationId = nom.NominationId,
                         });

            return await repositoryDropdownModel.GetPagedReponseAsync(new Pagination { SortOrderColumn = "Text" }, null, query);
        }

        public async Task<PagedList> GetAdmissionActiveUserDropdwon(long admissionId)
        {
            var admissionStatusId = AdmissionStatuses.Accepted.GetHashCode();
            Repository<NominationUserDropdownModel> repositoryDropdownModel = new(EduDbContext);
            var query = (from au in EduDbContext.AdmissionUsers
                         join nom in EduDbContext.Nominations on au.NominationId equals nom.NominationId
                         join user in EduDbContext.Users on nom.UserId equals user.UserId
                         where !nom.IsDeleted
                         && au.AdmissionId == admissionId
                         && au.IsConfirmByEmp == true
                         && au.AdmissionStatusId == admissionStatusId
                         && au.IsBondAccepted == true
                         select new NominationUserDropdownModel()
                         {
                             Id = nom.UserId,
                             AdmissionUserId = au.AdmissionUserId,
                             Text = user.FirstName + " " + user.LastName,
                             MsilUserId = user.MsilUserId,
                             Email = user.Email,
                             MobileNo = nom.MobileNo,
                             IsActive = nom.IsActive,
                         });

            return await repositoryDropdownModel.GetPagedReponseAsync(new Pagination { SortOrderColumn = "Text" }, null, query);
        }

        public async Task<List<string>> GetUserEmails(List<long> ids)
        {
            return await (from u in EduDbContext.Users
                          where !u.IsDeleted &&
                          ids.Contains(u.UserId)
                          select u.Email).ToListAsync();

        }

        public async Task<List<NominationUserDropdownModel>> GetUserLeadEmails(List<long> ids)
        {
            return await (from u in EduDbContext.Users
                          join um in EduDbContext.Users on u.ManagerId equals um.MsilUserId
                          where !u.IsDeleted
                          && u.ManagerId.HasValue
                          && ids.Contains(u.UserId)
                          select new NominationUserDropdownModel
                          {
                              Id = u.UserId,
                              Email = um.Email,

                          }).Distinct().ToListAsync();
        }
    }
}
