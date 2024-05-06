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
using System.Runtime.Intrinsics.Arm;

namespace Edu.Repository.Repositories
{
    public class LocationRepository : Repository<Location>, ILocationRepository
    {
        public LocationRepository(EduDbContext context)
            : base(context)
        { }

        private EduDbContext EduDbContext
        {
            get { return _dbContext as EduDbContext; }
        }

        public async Task<PagedList> GetLocationPaged(Pagination pagination)
        {
            string searchText = IsPropertyExist(pagination.Filters, "searchText") ? pagination.Filters?.searchText : null;

            IRepository<LocationModel> repository = new Repository<LocationModel>(EduDbContext);
            var query = (from loc in EduDbContext.Locations
                         where !loc.IsDeleted &&
                         (string.IsNullOrEmpty(searchText) || loc.LocationName.Contains(searchText))
                         select new LocationModel()
                         {
                             LocationId = loc.LocationId,
                             LocationName = loc.LocationName
                         });

            return await repository.GetPagedReponseAsync(pagination, null, query);
        }

        public async Task<PagedList> GetDropdwon(long? id = null, bool? isActive = null)
        {
            Repository<DropdownModel> repository = new(EduDbContext);
            var query = (from r in EduDbContext.Locations
                         where !r.IsDeleted &&
                         (!id.HasValue || r.LocationId == id) &&
                         (!isActive.HasValue || r.IsActive == isActive)
                         select new DropdownModel()
                         {
                             Id = r.LocationId,
                             Text = r.LocationName,
                             IsActive = r.IsActive
                         });

            return await repository.GetPagedReponseAsync(new Pagination { SortOrderColumn = "Text" }, null, query);
        }
    }
}
