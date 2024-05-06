using Core.Repository.Models;
using Core.Security;
using Core.Utility.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edu.Abstraction.ComplexModels;
using Edu.Abstraction.Models;
using Edu.Repository.Interfaces;
using Edu.Service.Interfaces;

namespace Edu.Service.Services
{
    public class BatchUserService : IBatchUserService
    {
        private readonly IBatchUserRepository _batchUserRepository;
        private readonly IUserProviderService _userProviderService;

        public BatchUserService(
            IBatchUserRepository batchUserRepository,
            IUserProviderService userProviderService
        )
        {
            _batchUserRepository = batchUserRepository;
            _userProviderService = userProviderService;
        }

        public async Task<ResponseModel> CreateBatchUser(BatchUser entity)
        {
            var existEntity = await GetBatchUserById(entity.BatchUserId);
            if (existEntity.Success)
            {
                return new ResponseModel
                {
                    Success = false,
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = "Batch User already exists.",
                };
            }

            entity.CreatedBy = _userProviderService.UserClaim.UserId;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            CommonUtils.EncodeProperties(entity);
            await _batchUserRepository.AddAsync(entity);
            var result = await _batchUserRepository.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Batch User created successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Batch User does not created." };
        }

        public async Task<ResponseModel> DeleteBatchUser(long id)
        {
            var entityResult = await GetBatchUserById(id);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as BatchUser;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            entity.IsDeleted = true;
            var result = await _batchUserRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Batch User deleted successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Batch User does not deleted." };
        }

        public async Task<ResponseModel> GetAllBatchUser(bool? isActive = null)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _batchUserRepository.Find(a => a.IsDeleted == false && (!isActive.HasValue || a.IsActive == isActive))
            };
        }

        public async Task<ResponseModel> GetBatchUserPaged(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _batchUserRepository.GetBatchUserPaged(pagination)
            };
        }

        public async Task<ResponseModel> GetBatchUserById(long id)
        {
            var result = await _batchUserRepository.SingleOrDefaultAsync(a => a.BatchUserId == id);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "Batch User does not exists." };
            }
        }

        public async Task<ResponseModel> UpdateBatchUser(BatchUser updateEntity)
        {
            var entityResult = await GetBatchUserById(updateEntity.BatchUserId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as BatchUser;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;

            CommonUtils.EncodeProperties(entity);
            var result = await _batchUserRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Batch User updated successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Batch User does not updated." };
        }

        public async Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null)
        {
            return (await _batchUserRepository.GetDropdwon(id, isActive)).Data;
        }
    }
}
