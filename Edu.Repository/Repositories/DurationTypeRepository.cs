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
    public class DurationTypeRepository : Repository<DurationType>, IDurationTypeRepository
    {
        public DurationTypeRepository(EduDbContext context)
            : base(context)
        { }

        private EduDbContext EduDbContext
        {
            get { return _dbContext as EduDbContext; }
        }

        public async Task<PagedList> GetDurationTypePaged(Pagination pagination)
        {
            bool? isActive = IsPropertyExist(pagination.Filters, "isActive") ? pagination.Filters?.isActive : null;

            IRepository<DurationTypeModel> repository = new Repository<DurationTypeModel>(EduDbContext);
            var query = (from r in EduDbContext.DurationTypes
                         where !r.IsDeleted 
                         && (!isActive.HasValue || isActive.Value == r.IsActive)
                         select new DurationTypeModel()
                         {
                             DurationTypeId = r.DurationTypeId,
                             IsActive = r.IsActive
                         });

            return await repository.GetPagedReponseAsync(pagination, null, query);
        }

        public async Task<PagedList> GetDropdwon(long? id = null, bool? isActive = null)
        {
            Repository<DropdownModel> repository = new(EduDbContext);
            var query = (from r in EduDbContext.DurationTypes
                         where !r.IsDeleted &&
                         (!id.HasValue || r.DurationTypeId == id) &&
                         (!isActive.HasValue || r.IsActive == isActive)
                         select new DropdownModel()
                         {
                             Id = r.DurationTypeId,
                             Text = r.DurationTypeName,
                             IsActive = r.IsActive
                         });

            return await repository.GetPagedReponseAsync(new Pagination { SortOrderColumn = "Text" }, null, query);
        }
    }
}
