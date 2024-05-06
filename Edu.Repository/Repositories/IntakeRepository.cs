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
using Microsoft.EntityFrameworkCore;

namespace Edu.Repository.Repositories
{
    public class IntakeRepository : Repository<Intake>, IIntakeRepository
    {
        public IntakeRepository(EduDbContext context)
            : base(context)
        { }

        private EduDbContext EduDbContext
        {
            get { return _dbContext as EduDbContext; }
        }

        public async Task<PagedList> GetIntakePaged(Pagination pagination)
        {
            string searchValue = IsPropertyExist(pagination.Filters, "searchValue") ? pagination.Filters?.searchValue : null;
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

            IRepository<IntakeModel> repository = new Repository<IntakeModel>(EduDbContext);
            var query = (from intake in EduDbContext.Intakes
                         join c in EduDbContext.Schemes on intake.SchemeId equals c.SchemeId
                         where !intake.IsDeleted &&
                         (string.IsNullOrEmpty(searchValue) || intake.IntakeName.Contains(searchValue))
                         && (!schemeId.HasValue || schemeId.Value == intake.SchemeId)
                         && (!intakeId.HasValue || intakeId.Value == intake.IntakeId)
                         && (!_startDate.HasValue || _startDate.Value.Date <= intake.StartDate.Date)
                         && (!_endDate.HasValue || _endDate.Value.Date >= intake.StartDate.Date)
                         select new IntakeModel()
                         {
                             IntakeId = intake.IntakeId,
                             IntakeName = intake.IntakeName,
                             StartDate = intake.StartDate,
                             SchemeName = c.SchemeName,
                             NominationCutoffDate = intake.NominationCutoffDate,
                             ExamDate = intake.ExamDate,
                             Institutes = (from ci in EduDbContext.IntakeInstitutes
                                           join ins in EduDbContext.Institutes on ci.InstituteId equals ins.InstituteId
                                           where ci.IntakeId == intake.IntakeId
                                           select new IntakeInstituteGridModel
                                           {
                                               InstituteName = ins.InstituteName,
                                               InstituteCode = ins.InstituteCode,
                                               AdmissionCutoffDate = ci.AdmissionCutoffDate,
                                               TotalSeats = ci.TotalSeats,
                                           }).ToList(),
                             DocumentTypes = (from ci in EduDbContext.IntakeDocumentTypes
                                              join ins in EduDbContext.DocumentTypes on ci.DocumentTypeId equals ins.DocumentTypeId
                                              where ci.IntakeId == intake.IntakeId
                                              select ins.DocumentTypeName).ToList(),
                         });

            return await repository.GetPagedReponseAsync(pagination, null, query);
        }

        public async Task<PagedList> GetDropdwon(long? id = null, long? schemeId = null, bool? isActive = null)
        {
            Repository<DropdownModel> repository = new(EduDbContext);
            var query = (from r in EduDbContext.Intakes
                         where !r.IsDeleted &&
                         (!id.HasValue || r.IntakeId == id) &&
                         (!isActive.HasValue || r.IsActive == isActive) &&
                         (!schemeId.HasValue || r.SchemeId == schemeId)
                         select new DropdownModel()
                         {
                             Id = r.IntakeId,
                             Text = r.IntakeName,
                             IsActive = r.IsActive
                         });

            return await repository.GetPagedReponseAsync(new Pagination { SortOrderColumn = "Id", SortOrderBy = "DESC" }, null, query);
        }

        public async Task<int> GetIntakeNameCount(string intakeName)
        {
            return await EduDbContext.Intakes.CountAsync(intake => !intake.IsDeleted && intake.IntakeName.Contains(intakeName));
        }

        public async Task<PagedList> GetEmployeeDropdwon(long schemeId, long userId)
        {
            Repository<DropdownModel> repository = new(EduDbContext);
            var query = (from intake in EduDbContext.Intakes
                         join nom in EduDbContext.Nominations on intake.IntakeId equals nom.IntakeId
                         where !intake.IsDeleted
                         && nom.UserId == userId
                         && intake.SchemeId == schemeId
                         select new DropdownModel()
                         {
                             Id = intake.IntakeId,
                             Text = intake.IntakeName,
                             IsActive = intake.IsActive
                         }).Distinct();

            return await repository.GetPagedReponseAsync(new Pagination { SortOrderColumn = "Id", SortOrderBy = "DESC" }, null, query);
        }
    }
}
