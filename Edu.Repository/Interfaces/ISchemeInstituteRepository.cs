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
    public interface ISchemeInstituteRepository : IRepository<SchemeInstitute>
    {
        Task<PagedList> GetSchemeInstitutePaged(Pagination pagination);
        Task<PagedList> GetDropdwon(long? id = null, long? schemeId = null);
    }
}