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
    public interface IAdmissionStatusRepository : IRepository<AdmissionStatus>
    {
        Task<PagedList> GetAdmissionStatusPaged(Pagination pagination);
        Task<PagedList> GetDropdwon(long? id = null, bool? isActive = null);
    }
}
