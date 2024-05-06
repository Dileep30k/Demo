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
    public interface IAdmissionRepository : IRepository<Admission>
    {
        Task<PagedList> GetAdmissionPaged(UserClaim userClaim, Pagination pagination);
        Task<PagedList> GetDropdwon(long? id = null, bool? isActive = null);
        Task<Admission> GetAdmission(UserClaim userClaim, long schemeId, long intakeId, long instituteId, bool isGts);
        Task<PagedList> GetAdmissionInstitutes(UserClaim userClaim, Pagination pagination);
        Task<PagedList> GetAdmissionUsers(Pagination pagination);
        Task<PagedList> GetConfirmAdmissionUsers(Pagination pagination);
        Task<int> GetTotalAdmission(dynamic filters);
        Task<int> GetTotalWaillist(dynamic filters);
        Task<int> GetPendingServiceAgreement(dynamic filters);
        Task<int> GetPendingAdmissionConfirmation(dynamic filters);
        Task<PagedList> GetAdmissionPagedByDesg(Pagination pagination);
        Task<PagedList> GetAdmissionPagedByDiv(Pagination pagination);
    }
}
