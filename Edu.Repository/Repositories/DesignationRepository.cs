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
    public class DesignationRepository : Repository<Designation>, IDesignationRepository
    {
        public DesignationRepository(EduDbContext context)
            : base(context)
        { }

        private EduDbContext EduDbContext
        {
            get { return _dbContext as EduDbContext; }
        }

        public async Task<PagedList> GetDesignationPaged(Pagination pagination)
        {
            string searchText = IsPropertyExist(pagination.Filters, "searchText") ? pagination.Filters?.searchText : null;

            IRepository<DesignationModel> repository = new Repository<DesignationModel>(EduDbContext);
            var query = (from deg in EduDbContext.Designations
                         where !deg.IsDeleted &&
                         (string.IsNullOrEmpty(searchText) || deg.DesignationName.Contains(searchText))
                         select new DesignationModel()
                         {
                             DesignationId = deg.DesignationId,
                             DesignationName = deg.DesignationName
                         });

            return await repository.GetPagedReponseAsync(pagination, null, query);
        }

        public async Task<PagedList> GetDropdwon(long? id = null, bool? isActive = null)
        {
            Repository<DropdownModel> repository = new(EduDbContext);
            var query = (from r in EduDbContext.Designations
                         where !r.IsDeleted &&
                         (!id.HasValue || r.DesignationId == id) &&
                         (!isActive.HasValue || r.IsActive == isActive)
                         select new DropdownModel()
                         {
                             Id = r.DesignationId,
                             Text = r.DesignationName,
                             IsActive = r.IsActive
                         });

            return await repository.GetPagedReponseAsync(new Pagination { SortOrderColumn = "Text" }, null, query);
        }
    }
}
