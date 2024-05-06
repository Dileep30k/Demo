using Core.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edu.Abstraction.ComplexModels;
using Edu.Abstraction.Models;

namespace Edu.Service.Interfaces
{
    public interface INominationService
    {
        Task<ResponseModel> GetNominationById(long id);
        Task<ResponseModel> GetAllNomination(bool? isActive = null);
        Task<ResponseModel> GetEligibilitiesPaged(Pagination pagination);
        Task<ResponseModel> GetNominationPaged(Pagination pagination);
        Task<ResponseModel> GetNominationApproverPaged(Pagination pagination);
        Task<ResponseModel> CreateNomination(Nomination entity);
        Task<ResponseModel> CreateNominations(ICollection<Nomination> entities);
        Task<ResponseModel> UpdateNomination(Nomination updateEntity);
        Task<ResponseModel> UpdateNomination(NominationFormModel updateEntity);
        Task<ResponseModel> UpdateNomination(List<long> ids, int type, string remaks);
        Task<ResponseModel> UpdateNominationScore(Nomination updateEntity);
        Task<ResponseModel> UpdateNominationScores(List<Nomination> updateEntities);
        Task<ResponseModel> UpdateNominationStatus(Nomination updateEntity);
        Task<ResponseModel> UpdateNominationScoreApproval(Nomination updateEntity);
        Task<ResponseModel> DeleteNomination(long id);
        Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null);
        Task<ResponseModel> PublishEligibilities(long schemeId, long intakeId);
        Task<ResponseModel> GetNominationModel(long intakeId);
        Task<ResponseModel> GetScorecardPaged(Pagination pagination);
        Task<ResponseModel> GetScorecardModel(long intakeId);
        Task<bool> AllowGtsScorecard(long intakeId);
        Task<int> GetTotalEligible(dynamic filters);
        Task<int> GetTotalNomination(dynamic filters);
        Task<int> GetTotalInstitute(dynamic filters);
        Task<int> GetPendingNominationAcceptance(dynamic filters);
        Task<ResponseModel> GetEmployeeNomination(long schemeId, long intakeId, long userId);
        Task<List<long>> GetNominationUsers(long schemeId, long intakeId);
        Task PendingScorecardReminderEmails();
        Task NominationApprover1ReminderEmails();
    }
}
