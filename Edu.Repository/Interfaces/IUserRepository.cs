using Core.Repository;
using Core.Repository.Models;
using Edu.Abstraction.ComplexModels;
using Edu.Abstraction.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Edu.Repository.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<PagedList> GetUserPaged(Pagination pagination);
        Task<PagedList> GetDropdwon(long? id = null, bool? isActive = null);
        Task<PagedList> GetUserDropdwon(long? id = null, bool? isActive = null);
        Task<PagedList> GetNominationUserDropdwon(long schemeId, long intakeId, long instituteId);
        Task<PagedList> GetAdmissionActiveUserDropdwon(long admissionId);
        Task<List<string>> GetUserEmails(List<long> ids);
        Task<List<NominationUserDropdownModel>> GetUserLeadEmails(List<long> ids);
        Task<PagedList> GetScorecardUserDropdwon(long schemeId, long intakeId);
    }
}
