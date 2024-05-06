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
    public class SchemeInstituteRepository : Repository<SchemeInstitute>, ISchemeInstituteRepository
    {
        public SchemeInstituteRepository(EduDbContext context)
            : base(context)
        { }

        private EduDbContext EduDbContext
        {
            get { return _dbContext as EduDbContext; }
        }

        public async Task<PagedList> GetSchemeInstitutePaged(Pagination pagination)
        {
            long? schemeId = IsPropertyExist(pagination.Filters, "schemeId") ? pagination.Filters?.schemeId : null;

            IRepository<SchemeInstituteModel> repository = new Repository<SchemeInstituteModel>(EduDbContext);
            var query = (from c in EduDbContext.SchemeInstitutes
                         join ins in EduDbContext.Institutes on c.InstituteId equals ins.InstituteId
                         where !c.IsDeleted
                         && (!schemeId.HasValue || schemeId.Value == c.SchemeId)
                         select new SchemeInstituteModel()
                         {
                             SchemeInstituteId = c.SchemeInstituteId,
                             InstituteName = ins.InstituteName,
                             IsActive = c.IsActive
                         });

            return await repository.GetPagedReponseAsync(pagination, null, query);
        }

        public async Task<PagedList> GetDropdwon(long? id = null, long? schemeId = null)
        {
            Repository<DropdownModel> repository = new(EduDbContext);
            var query = (from c in EduDbContext.SchemeInstitutes
                         join ins in EduDbContext.Institutes on c.InstituteId equals ins.InstituteId
                         where !c.IsDeleted &&
                         (!id.HasValue || c.SchemeInstituteId == id) &&
                         (!schemeId.HasValue || c.SchemeId == schemeId)
                         select new DropdownModel()
                         {
                             Id = ins.InstituteId,
                             Text = ins.InstituteName,
                             IsActive = ins.IsActive
                         });

            return await repository.GetPagedReponseAsync(new Pagination { SortOrderColumn = "Text" }, null, query);
        }
    }
}
