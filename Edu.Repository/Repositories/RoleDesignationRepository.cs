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
    public class RoleDesignationRepository : Repository<RoleDesignation>, IRoleDesignationRepository
    {
        public RoleDesignationRepository(EduDbContext context)
            : base(context)
        { }

        private EduDbContext EduDbContext
        {
            get { return _dbContext as EduDbContext; }
        }

        public async Task<PagedList> GetRoleDesignationPaged(Pagination pagination)
        {
            long? roleId = IsPropertyExist(pagination.Filters, "roleId") ? pagination.Filters?.roleId : null;

            IRepository<RoleDesignationModel> repository = new Repository<RoleDesignationModel>(EduDbContext);
            var query = (from r in EduDbContext.RoleDesignations
                         join des in EduDbContext.Designations on r.DesignationId equals des.DesignationId
                         where !r.IsDeleted
                         && (!roleId.HasValue || roleId.Value == r.RoleId)
                         select new RoleDesignationModel()
                         {
                             RoleDesignationId = r.RoleDesignationId,
                             DesignationName = des.DesignationName
                         });

            return await repository.GetPagedReponseAsync(pagination, null, query);
        }

        public async Task<PagedList> GetDropdwon(long? id = null, bool? isActive = null)
        {
            Repository<DropdownModel> repository = new(EduDbContext);
            var query = (from r in EduDbContext.RoleDesignations
                         where !r.IsDeleted &&
                         (!id.HasValue || r.RoleDesignationId == id) &&
                         (!isActive.HasValue || r.IsActive == isActive)
                         select new DropdownModel()
                         {
                             Id = r.RoleDesignationId,
                             IsActive = r.IsActive
                         });

            return await repository.GetPagedReponseAsync(new Pagination { SortOrderColumn = "Text" }, null, query);
        }

        public async Task<List<long>> GetRolesByDesignationId(long designationId)
        {
            return await (from r in EduDbContext.RoleDesignations
                          where !r.IsDeleted &&
                          (r.DesignationId == designationId)
                          select r.RoleId)
                          .Distinct()
                          .OrderBy(r => r)
                          .ToListAsync();
        }
    }
}
