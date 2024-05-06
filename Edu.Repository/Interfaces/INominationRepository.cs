using Core.Repository;
using Core.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edu.Abstraction.Models;
using Edu.Abstraction.ComplexModels;

namespace Edu.Repository.Interfaces
{
    public interface INominationRepository : IRepository<Nomination>
    {
        Task<PagedList> GetEligibilitiesPaged(UserClaim userClaim, Pagination pagination);
        Task<PagedList> GetNominationPaged(UserClaim userClaim, Pagination pagination);
        Task<PagedList> GetNominationApproverPaged(UserClaim userClaim, Pagination pagination);
        Task<PagedList> GetDropdwon(long? id = null, bool? isActive = null);
        Task<NominationFormModel> GetNominationModel(UserClaim userClaim, long intakeId);
        Task<PagedList> GetScorecardPaged(Pagination pagination);
        Task<NominationFormModel> GetScorecardModel(UserClaim userClaim, long intakeId);
        Task<bool> AllowGtsScorecard(UserClaim userClaim, long intakeId);
        Task<int> GetTotalEligible(dynamic filters);
        Task<int> GetTotalNomination(dynamic filters);
        Task<int> GetTotalInstitute(dynamic filters);
        Task<int> GetPendingNominationAcceptance(dynamic filters);
        Task<List<NominationUserDropdownModel>> GetAllPendingScorecardUser(List<long> intakeIds);
        Task<Nomination> GetEmployeeNomination(long schemeId, long intakeId, long userId);
        Task<List<long>> GetNominationUsers(long schemeId, long intakeId);
        Task<List<NominationApproverEmailModel>> GetNominationApprover1();
        Task<List<NominationApproverEmailModel>> GetNominationApprover2(List<long> ids);
    }
}
