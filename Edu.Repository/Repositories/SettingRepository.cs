using Core.Repository;
using Core.Repository.Models;
using Core.Utility.Utils;
using Edu.Abstraction.ComplexModels;
using Edu.Abstraction.Models;
using Edu.Repository.Contexts;
using Edu.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edu.Repository.Repositories
{
    public class SettingRepository : Repository<Setting>, ISettingRepository
    {
        public SettingRepository(EduDbContext context)
            : base(context)
        { }

        private EduDbContext EduDbContext
        {
            get { return _dbContext as EduDbContext; }
        }

        public async Task<PagedList> GetSettingPaged(Pagination pagination)
        {
            bool? isActive = IsPropertyExist(pagination.Filters, "isActive") ? pagination.Filters?.isActive : null;

            IRepository<SettingModel> repositorySettingModel = new Repository<SettingModel>(EduDbContext);
            var query = (from us in EduDbContext.Settings
                         where us.IsDeleted == false &&
                         (!isActive.HasValue || isActive.Value == us.IsActive)
                         select new SettingModel()
                         {
                             SettingId = us.SettingId,
                             SettingName = us.SettingName,
                             SettingValue = us.SettingValue,
                             IsActive = us.IsActive,
                         });

            return await repositorySettingModel.GetPagedReponseAsync(pagination, null, query);
        }

        public async Task<List<Setting>> GetSettingsByKeys(List<string> keys)
        {
            return await (from s in EduDbContext.Settings
                          where keys.Contains(s.SettingKey)
                          select s)
                         .ToListAsync();
        }
    }
}
