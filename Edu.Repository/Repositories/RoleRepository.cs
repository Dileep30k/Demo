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
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(EduDbContext context)
            : base(context)
        { }

        private EduDbContext EduDbContext
        {
            get { return _dbContext as EduDbContext; }
        }

        public async Task<PagedList> GetRolePaged(Pagination pagination)
        {
            string searchText = IsPropertyExist(pagination.Filters, "searchText") ? pagination.Filters?.searchText : null;

            IRepository<RoleModel> repository = new Repository<RoleModel>(EduDbContext);
            var query = (from r in EduDbContext.Roles
                         where !r.IsDeleted &&
                         (string.IsNullOrEmpty(searchText) || r.RoleName.Contains(searchText))
                         select new RoleModel()
                         {
                             RoleId = r.RoleId,
                             RoleName = r.RoleName,
                             Designations = string.Join(", ", (from rd in EduDbContext.RoleDesignations
                                             join des in EduDbContext.Designations on rd.DesignationId equals des.DesignationId
                                             where rd.RoleId == r.RoleId
                                             select des.DesignationName)),
                         });

            return await repository.GetPagedReponseAsync(pagination, null, query);
    }

    public async Task<PagedList> GetDropdwon(long? id = null, bool? isActive = null)
    {
        Repository<DropdownModel> repository = new(EduDbContext);
        var query = (from r in EduDbContext.Roles
                     where !r.IsDeleted &&
                     (!id.HasValue || r.RoleId == id) &&
                     (!isActive.HasValue || r.IsActive == isActive)
                     select new DropdownModel()
                     {
                         Id = r.RoleId,
                         Text = r.RoleName,
                         IsActive = r.IsActive
                     });

        return await repository.GetPagedReponseAsync(new Pagination { SortOrderColumn = "Text" }, null, query);
    }
}
}
