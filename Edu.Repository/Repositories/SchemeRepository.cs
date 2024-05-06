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

namespace Edu.Repository.Repositories
{
    public class SchemeRepository : Repository<Scheme>, ISchemeRepository
    {
        public SchemeRepository(EduDbContext context)
            : base(context)
        { }

        private EduDbContext EduDbContext
        {
            get { return _dbContext as EduDbContext; }
        }

        public async Task<PagedList> GetSchemePaged(Pagination pagination)
        {
            string searchValue = IsPropertyExist(pagination.Filters, "searchValue") ? pagination.Filters?.searchValue : null;
            long? instituteId = IsPropertyExist(pagination.Filters, "instituteId") ? pagination.Filters?.instituteId : null;

            IRepository<SchemeModel> repository = new Repository<SchemeModel>(EduDbContext);
            var query = (from sc in EduDbContext.Schemes
                         join dt in EduDbContext.DurationTypes on sc.DurationTypeId equals dt.DurationTypeId
                         where !sc.IsDeleted &&
                         (string.IsNullOrEmpty(searchValue) || sc.SchemeName.Contains(searchValue))
                         && (!instituteId.HasValue || EduDbContext.SchemeInstitutes
                                      .Where(si => si.SchemeId == sc.SchemeId && instituteId.Value == si.InstituteId)
                                      .Select(i => i.SchemeId).Contains(sc.SchemeId))
                         select new SchemeModel()
                         {
                             SchemeId = sc.SchemeId,
                             SchemeName = sc.SchemeName,
                             SchemeCode = sc.SchemeCode,
                             Duration = $"{sc.Duration} {dt.DurationTypeName}",
                             Institutes = (from ci in EduDbContext.SchemeInstitutes
                                           join ins in EduDbContext.Institutes on ci.InstituteId equals ins.InstituteId
                                           where ci.SchemeId == sc.SchemeId
                                           select ins.InstituteName).ToList(),
                         });

            return await repository.GetPagedReponseAsync(pagination, null, query);
        }

        public async Task<PagedList> GetDropdwon(long? id = null, bool? isActive = null)
        {
            Repository<DropdownModel> repository = new(EduDbContext);
            var query = (from r in EduDbContext.Schemes
                         where !r.IsDeleted &&
                         (!id.HasValue || r.SchemeId == id) &&
                         (!isActive.HasValue || r.IsActive == isActive)
                         select new DropdownModel()
                         {
                             Id = r.SchemeId,
                             Text = r.SchemeName,
                             IsActive = r.IsActive
                         });

            return await repository.GetPagedReponseAsync(new Pagination { SortOrderColumn = "Text" }, null, query);
        }

        public async Task<PagedList> GetEmployeeDropdwon(long userId)
        {
            Repository<DropdownModel> repository = new(EduDbContext);
            var query = (from scheme in EduDbContext.Schemes
                         join nom in EduDbContext.Nominations on scheme.SchemeId equals nom.SchemeId
                         where !scheme.IsDeleted
                         && nom.UserId == userId
                         select new DropdownModel()
                         {
                             Id = scheme.SchemeId,
                             Text = scheme.SchemeName,
                             IsActive = scheme.IsActive
                         }).Distinct();

            return await repository.GetPagedReponseAsync(new Pagination { SortOrderColumn = "Id", SortOrderBy = "DESC" }, null, query);
        }
    }
}
