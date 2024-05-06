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
    public class IntakeLocationRepository : Repository<IntakeLocation>, IIntakeLocationRepository
    {
        public IntakeLocationRepository(EduDbContext context)
            : base(context)
        { }

        private EduDbContext EduDbContext
        {
            get { return _dbContext as EduDbContext; }
        }

        public async Task<PagedList> GetIntakeLocationPaged(Pagination pagination)
        {
            long? intakeId = IsPropertyExist(pagination.Filters, "intakeId") ? pagination.Filters?.intakeId : null;

            IRepository<IntakeLocationModel> repository = new Repository<IntakeLocationModel>(EduDbContext);
            var query = (from bi in EduDbContext.IntakeLocations
                         join ins in EduDbContext.Locations on bi.LocationId equals ins.LocationId
                         where !bi.IsDeleted 
                         && (!intakeId.HasValue || intakeId.Value == bi.IntakeId)
                         select new IntakeLocationModel()
                         {
                             IntakeLocationId = bi.IntakeLocationId,
                             LocationName = ins.LocationName,
                         });

            return await repository.GetPagedReponseAsync(pagination, null, query);
        }

        public async Task<PagedList> GetDropdwon(long? id = null, bool? isActive = null)
        {
            Repository<DropdownModel> repository = new(EduDbContext);
            var query = (from r in EduDbContext.IntakeLocations
                         join ins in EduDbContext.Locations on r.LocationId equals ins.LocationId
                         where !r.IsDeleted &&
                         (!id.HasValue || r.IntakeLocationId == id) &&
                         (!isActive.HasValue || r.IsActive == isActive)
                         select new DropdownModel()
                         {
                             Id = r.IntakeLocationId,
                             Text = ins.LocationName,
                             IsActive = r.IsActive
                         });

            return await repository.GetPagedReponseAsync(new Pagination { SortOrderColumn = "Text" }, null, query);
        }
    }
}
