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
    public class DurationTypeService : IDurationTypeService
    {
        private readonly IDurationTypeRepository _durationTypeRepository;
        private readonly IUserProviderService _userProviderService;

        public DurationTypeService(
            IDurationTypeRepository durationTypeRepository,
            IUserProviderService userProviderService
        )
        {
            _durationTypeRepository = durationTypeRepository;
            _userProviderService = userProviderService;
        }

        public async Task<ResponseModel> CreateDurationType(DurationType entity)
        {
            var existEntity = await GetDurationTypeById(entity.DurationTypeId);
            if (existEntity.Success)
            {
                return new ResponseModel
                {
                    Success = false,
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = "Duration Type already exists.",
                };
            }

            entity.CreatedBy = _userProviderService.UserClaim.UserId;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            CommonUtils.EncodeProperties(entity);
            await _durationTypeRepository.AddAsync(entity);
            var result = await _durationTypeRepository.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Duration Type created successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Duration Type does not created." };
        }

        public async Task<ResponseModel> DeleteDurationType(long id)
        {
            var entityResult = await GetDurationTypeById(id);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as DurationType;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            entity.IsDeleted = true;
            var result = await _durationTypeRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Duration Type deleted successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Duration Type does not deleted." };
        }

        public async Task<ResponseModel> GetAllDurationType(bool? isActive = null)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _durationTypeRepository.Find(a => a.IsDeleted == false && (!isActive.HasValue || a.IsActive == isActive))
            };
        }

        public async Task<ResponseModel> GetDurationTypePaged(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _durationTypeRepository.GetDurationTypePaged(pagination)
            };
        }

        public async Task<ResponseModel> GetDurationTypeById(long id)
        {
            var result = await _durationTypeRepository.SingleOrDefaultAsync(a => a.DurationTypeId == id);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "Duration Type does not exists." };
            }
        }

        public async Task<ResponseModel> UpdateDurationType(DurationType updateEntity)
        {
            var entityResult = await GetDurationTypeById(updateEntity.DurationTypeId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as DurationType;
            entity.DurationTypeName = updateEntity.DurationTypeName;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;

            CommonUtils.EncodeProperties(entity);
            var result = await _durationTypeRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Duration Type updated successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Duration Type does not updated." };
        }

        public async Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null)
        {
            return (await _durationTypeRepository.GetDropdwon(id, isActive)).Data;
        }
    }
}
