using Core.Repository.Models;
using Core.Security;
using Core.Utility.Utils;
using Edu.Abstraction.ComplexModels;
using Edu.Abstraction.Models;
using Edu.Repository.Interfaces;
using Edu.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edu.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly ITokenBuilder _tokenBuilder;
        private readonly IUserProviderService _userProviderService;
        private readonly IUserLoginService _userLoginService;

        public UserService(
            IConfiguration configuration,
            ITokenBuilder tokenBuilder,
            IUserRepository userRepository,
            IUserLoginService userLoginService,
            IUserProviderService userProviderService
        )
        {
            _configuration = configuration;
            _tokenBuilder = tokenBuilder;
            _userRepository = userRepository;
            _userLoginService = userLoginService;
            _userProviderService = userProviderService;
        }

        public async Task<ResponseModel> CreateUser(User entity)
        {
            var existEntity = await GetUserByMsilUserId(entity.MsilUserId);
            if (existEntity.Success)
            {
                return new ResponseModel
                {
                    Success = false,
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = "User email already exists.",
                };
            }

            entity.CreatedBy = _userProviderService.UserClaim.UserId;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;


            CommonUtils.EncodeProperties(entity);
            await _userRepository.AddAsync(entity);
            var result = await _userRepository.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "User created successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "User does not created." };
        }

        public async Task<ResponseModel> CreateUsers(List<User> entities)
        {
            foreach (var entity in entities)
            {
                entity.CreatedBy = _userProviderService.UserClaim.UserId;
                entity.UpdatedBy = _userProviderService.UserClaim.UserId;
                CommonUtils.EncodeProperties(entity);
                await _userRepository.AddAsync(entity);
            }
            var result = await _userRepository.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Users created successfully."
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Users does not created." };
        }

        public async Task<ResponseModel> DeleteUser(long id)
        {
            var entityResult = await GetUserById(id);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as User;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            entity.IsDeleted = true;
            var result = await _userRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "User deleted successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "User does not deleted." };
        }

        public async Task<ResponseModel> GetAllUser(bool? isActive = null)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _userRepository.Find(a => a.IsDeleted == false && (!isActive.HasValue || a.IsActive == isActive))
            };
        }

        public async Task<ResponseModel> GetUserPaged(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _userRepository.GetUserPaged(pagination)
            };
        }

        public async Task<ResponseModel> GetUserById(long id)
        {
            var result = await _userRepository.SingleOrDefaultAsync(a => a.UserId == id);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "User does not exists." };
            }
        }

        public async Task<ResponseModel> GetUserByMsilUserId(long msilUserId)
        {
            var result = (await _userRepository.SingleOrDefaultAsync(a => a.MsilUserId == msilUserId));

            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "User does not exists." };
            }
        }

        public async Task<ResponseModel> UpdateUser(User updateEntity)
        {
            var entityResult = await GetUserById(updateEntity.UserId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as User;
            entity.Email = updateEntity.Email;
            entity.FirstName = updateEntity.FirstName;
            entity.LastName = updateEntity.LastName;
            entity.MobileNo = updateEntity.MobileNo;
            entity.MsilUserId = updateEntity.MsilUserId;
            entity.MsilUserName = updateEntity.MsilUserName;
            entity.Doj = updateEntity.Doj;
            entity.Dos = updateEntity.Dos;
            entity.Dol = updateEntity.Dol;
            entity.VerticalId = updateEntity.VerticalId;
            entity.DivisionId = updateEntity.DivisionId;
            entity.DesignationId = updateEntity.DesignationId;
            entity.DepartmentId = updateEntity.DepartmentId;
            entity.LocationId = updateEntity.LocationId;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;

            CommonUtils.EncodeProperties(entity);
            var result = await _userRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "User updated successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "User does not updated." };
        }

        public async Task<ResponseModel> AuthenticateUser(LoginModel model)
        {
            ResponseModel entityResult = await AuthenticateSystemUser(model);
            if (!entityResult.Success) { return entityResult; }

            var user = entityResult.Data as User;

            if (bool.Parse(_configuration.GetSection("SingleUserLogin").Value))
            {
                var sessionId = Guid.NewGuid().ToString();

                if (!model.ForceLogin)
                {
                    var userLoginResult = await _userLoginService.GetUserLoginByMsilUserId(model.MsilUserId);
                    if (userLoginResult.Success)
                    {
                        if (userLoginResult.Data.LogInExpireTime > DateTime.Now)
                        {
                            return new ResponseModel
                            {
                                Success = false,
                                StatusCode = StatusCodes.Status403Forbidden,
                                Message = "Another session for user is active.",
                            };
                        }
                    }
                }

                await _userLoginService.CreateUserLogin(new UserLogin
                {
                    MsilUserId = model.MsilUserId,
                    SessionId = sessionId,
                    LogInExpireTime = DateTime.Now.AddMinutes(double.Parse(_configuration.GetSection("JwtConfig").GetSection("expirationInMinutes").Value)),
                });
            }
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "User Login successfully.",
                Data = user
            };
        }

        public async Task<ResponseModel> AuthenticateMsilUser(LoginModel model)
        {

            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "User Validation Error" };
        }

        public async Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null)
        {
            return (await _userRepository.GetDropdwon(id, isActive)).Data;
        }

        public async Task<List<UserDropdownModel>> GetUserDropdwon(long? id = null, bool? isActive = null)
        {
            return (await _userRepository.GetUserDropdwon(id, isActive)).Data;
        }

        public async Task<List<NominationUserDropdownModel>> GetNominationUserDropdwon(long schemeId, long intakeId, long instituteId)
        {
            return (await _userRepository.GetNominationUserDropdwon(schemeId, intakeId, instituteId)).Data;
        }

        public async Task<List<NominationUserDropdownModel>> GetScorecardUserDropdwon(long schemeId, long intakeId)
        {
            return (await _userRepository.GetScorecardUserDropdwon(schemeId, intakeId)).Data;
        }

        public async Task<List<NominationUserDropdownModel>> GetAdmissionActiveUserDropdwon(long admissionId)
        {
            return (await _userRepository.GetAdmissionActiveUserDropdwon(admissionId)).Data;
        }

        public async Task<List<string>> GetUserEmails(List<long> ids)
        {
            return await _userRepository.GetUserEmails(ids);
        }

        public async Task<List<NominationUserDropdownModel>> GetUserLeadEmails(List<long> ids)
        {
            return await _userRepository.GetUserLeadEmails(ids);
        }

        private async Task<ResponseModel> AuthenticateSystemUser(LoginModel model)
        {
            var entityResult = await GetUserByMsilUserId(model.MsilUserId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as User;

            if (entity.IsDeleted)
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "User does not exists." };
            }
            if (!entity.IsActive)
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status405MethodNotAllowed, Message = "User does not active." };
            }

            var hash = PasswordUtility.GenerateHash(model.Password, entity.Salt);
            if (hash != entity.Password)
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status406NotAcceptable, Message = "Password did not match." };
            }

            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "User Login successfully.",
                Data = entity
            };
        }
    }
}
