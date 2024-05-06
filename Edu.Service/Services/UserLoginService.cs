using Core.Repository.Models;
using Core.Security;
using Core.Utility.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edu.Abstraction.ComplexModels;
using Edu.Abstraction.Enums;
using Edu.Abstraction.Models;
using Edu.Repository.Interfaces;
using Edu.Service.Interfaces;

namespace Edu.Service.Services
{
    public class UserLoginService : IUserLoginService
    {
        private readonly IUserLoginRepository _userLoginRepository;
        private readonly IUserProviderService _userProviderService;

        public UserLoginService(
            IUserLoginRepository userLoginRepository,
            IUserProviderService userProviderService
        )
        {
            _userLoginRepository = userLoginRepository;
            _userProviderService = userProviderService;
        }

        public async Task<ResponseModel> CreateUserLogin(UserLogin entity)
        {
            var existEntity = (await _userLoginRepository.SingleOrDefaultAsync(a => a.MsilUserId == entity.MsilUserId));

            if (existEntity != null)
            {
                existEntity.SessionId = entity.SessionId;
                existEntity.LogInExpireTime = entity.LogInExpireTime;
                existEntity.UpdatedBy = _userProviderService.UserClaim.UserId;
                existEntity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            }
            else
            {
                entity.CreatedBy = _userProviderService.UserClaim.UserId;
                entity.UpdatedBy = _userProviderService.UserClaim.UserId;
                await _userLoginRepository.AddAsync(entity);
            }

            CommonUtils.EncodeProperties(entity);
            var result = await _userLoginRepository.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "User Login created successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "User Login does not created." };
        }

        public async Task<ResponseModel> DeleteUserLoginByEmail()
        {
            var entityResult = await GetUserLoginByMsilUserId(_userProviderService.UserClaim.MsilUserId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as UserLogin;

            _userLoginRepository.Remove(entity);

            var result = await _userLoginRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "User deleted successfully.",
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "User Login does not deleted." };
        }

        public async Task<ResponseModel> GetUserLoginById(long id)
        {
            var result = await _userLoginRepository.SingleOrDefaultAsync(a => a.UserLoginId == id);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "User Login does not exists." };
            }
        }

        public async Task<ResponseModel> GetUserLoginByMsilUserId(long msilUserId)
        {
            var result = (await _userLoginRepository.SingleOrDefaultAsync(a => a.MsilUserId == msilUserId));
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "User Login does not exists." };
            }
        }

        public async Task<ResponseModel> UpdateUserLogin(UserLogin updateEntity)
        {
            var entityResult = await GetUserLoginById(updateEntity.UserLoginId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as UserLogin;
            entity.IsActive = updateEntity.IsActive;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            CommonUtils.EncodeProperties(entity);
            var result = await _userLoginRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "User Login updated successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "User Login does not updated." };
        }
    }
}
