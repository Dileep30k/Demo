using Core.Repository;
using Core.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edu.Abstraction.Models;

namespace Edu.Repository.Interfaces
{
    public interface IBatchRepository : IRepository<Batch>
    {
        Task<PagedList> GetBatchPaged(Pagination pagination);
        Task<PagedList> GetDropdwon(long? id = null, long? schemeId = null, long? intakeId = null, long? instituteId = null);
        Task<int> GetBatchCount(Batch batch);
    }
}
