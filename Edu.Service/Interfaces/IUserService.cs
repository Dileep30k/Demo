using Core.Repository.Models;
using Edu.Abstraction.ComplexModels;
using Edu.Abstraction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edu.Service.Interfaces
{
    public interface IUserService
    {
        Task<ResponseModel> GetUserById(long id);
        Task<ResponseModel> GetUserByMsilUserId(long msilUserId);
        Task<ResponseModel> GetAllUser(bool? isActive = null);
        Task<ResponseModel> GetUserPaged(Pagination pagination);
        Task<ResponseModel> CreateUser(User entity);
        Task<ResponseModel> CreateUsers(List<User> entities);
        Task<ResponseModel> UpdateUser(User updateEntity);
        Task<ResponseModel> DeleteUser(long id);
        Task<ResponseModel> AuthenticateUser(LoginModel model);
        Task<ResponseModel> AuthenticateMsilUser(LoginModel model);
        Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null);
        Task<List<UserDropdownModel>> GetUserDropdwon(long? id = null, bool? isActive = null);
        Task<List<NominationUserDropdownModel>> GetNominationUserDropdwon(long schemeId, long intakeId, long instituteId);
        Task<List<NominationUserDropdownModel>> GetAdmissionActiveUserDropdwon(long admissionId);
        Task<List<string>> GetUserEmails(List<long> ids);
        Task<List<NominationUserDropdownModel>> GetUserLeadEmails(List<long> ids);
        Task<List<NominationUserDropdownModel>> GetScorecardUserDropdwon(long schemeId, long intakeId);
    }
}
