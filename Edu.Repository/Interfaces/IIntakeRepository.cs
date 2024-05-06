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
    public interface IIntakeRepository : IRepository<Intake>
    {
        Task<PagedList> GetIntakePaged(Pagination pagination);
        Task<PagedList> GetDropdwon(long? id = null, long? schemeId = null, bool ? isActive = null);
        Task<int> GetIntakeNameCount(string intakeName);
        Task<PagedList> GetEmployeeDropdwon(long schemeId, long userId);
    }
}
