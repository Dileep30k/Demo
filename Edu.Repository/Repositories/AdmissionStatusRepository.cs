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
    public class AdmissionStatusRepository : Repository<AdmissionStatus>, IAdmissionStatusRepository
    {
        public AdmissionStatusRepository(EduDbContext context)
            : base(context)
        { }

        private EduDbContext EduDbContext
        {
            get { return _dbContext as EduDbContext; }
        }

        public async Task<PagedList> GetAdmissionStatusPaged(Pagination pagination)
        {
            bool? isActive = IsPropertyExist(pagination.Filters, "isActive") ? pagination.Filters?.isActive : null;

            IRepository<AdmissionStatusModel> repository = new Repository<AdmissionStatusModel>(EduDbContext);
            var query = (from r in EduDbContext.AdmissionStatuses
                         where !r.IsDeleted 
                         && (!isActive.HasValue || isActive.Value == r.IsActive)
                         select new AdmissionStatusModel()
                         {
                             AdmissionStatusId = r.AdmissionStatusId,
                             IsActive = r.IsActive
                         });

            return await repository.GetPagedReponseAsync(pagination, null, query);
        }

        public async Task<PagedList> GetDropdwon(long? id = null, bool? isActive = null)
        {
            Repository<DropdownModel> repository = new(EduDbContext);
            var query = (from r in EduDbContext.AdmissionStatuses
                         where !r.IsDeleted &&
                         (!id.HasValue || r.AdmissionStatusId == id) &&
                         (!isActive.HasValue || r.IsActive == isActive)
                         select new DropdownModel()
                         {
                             Id = r.AdmissionStatusId,
                             Text = r.AdmissionStatusName,
                             IsActive = r.IsActive
                         });

            return await repository.GetPagedReponseAsync(new Pagination { SortOrderColumn = "Text" }, null, query);
        }
    }
}
