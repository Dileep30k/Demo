using Core.Repository;
using Core.Repository.Models;
using Edu.Abstraction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edu.Repository.Interfaces
{
    public interface ISettingRepository : IRepository<Setting>
    {
        Task<PagedList> GetSettingPaged(Pagination pagination);
        Task<List<Setting>> GetSettingsByKeys(List<string> keys);
    }
}
