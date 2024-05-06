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
using Edu.Repository.Repositories;

namespace Edu.Service.Services
{
    public class IntakeLocationService : IIntakeLocationService
    {
        private readonly IIntakeLocationRepository _intakeLocationRepository;
        private readonly IUserProviderService _userProviderService;

        public IntakeLocationService(
            IIntakeLocationRepository intakeLocationRepository,
            IUserProviderService userProviderService
        )
        {
            _intakeLocationRepository = intakeLocationRepository;
            _userProviderService = userProviderService;
        }

        public async Task<ResponseModel> CreateIntakeLocation(IntakeLocation entity)
        {
            var existEntity = await GetIntakeLocationByLocationId(entity.IntakeId, entity.LocationId);
            if (existEntity.Success)
            {
                return new ResponseModel
                {
                    Success = false,
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = "Intake Location already exists.",
                };
            }

            entity.CreatedBy = _userProviderService.UserClaim.UserId;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            CommonUtils.EncodeProperties(entity);
            await _intakeLocationRepository.AddAsync(entity);
            var result = await _intakeLocationRepository.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Intake Location created successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Intake Location does not created." };
        }

        public async Task<ResponseModel> DeleteIntakeLocation(long id)
        {
            var entityResult = await GetIntakeLocationById(id);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as IntakeLocation;
            _intakeLocationRepository.Remove(entity);
            var result = await _intakeLocationRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Intake Location deleted successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Intake Location does not deleted." };
        }

        public async Task<ResponseModel> GetAllIntakeLocation(bool? isActive = null)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _intakeLocationRepository.Find(a => a.IsDeleted == false && (!isActive.HasValue || a.IsActive == isActive))
            };
        }

        public async Task<ResponseModel> GetIntakeLocationPaged(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _intakeLocationRepository.GetIntakeLocationPaged(pagination)
            };
        }

        public async Task<ResponseModel> GetIntakeLocationById(long id)
        {
            var result = await _intakeLocationRepository.SingleOrDefaultAsync(a => a.IntakeLocationId == id);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "Intake Location does not exists." };
            }
        }

        public async Task<ResponseModel> UpdateIntakeLocation(IntakeLocation updateEntity)
        {
            var entityResult = await GetIntakeLocationById(updateEntity.IntakeLocationId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as IntakeLocation;
            entity.IntakeId = updateEntity.IntakeId;
            entity.LocationId = updateEntity.LocationId;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;

            CommonUtils.EncodeProperties(entity);
            var result = await _intakeLocationRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Intake Location updated successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Intake Location does not updated." };
        }

        public async Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null)
        {
            return (await _intakeLocationRepository.GetDropdwon(id, isActive)).Data;
        }

        private async Task<ResponseModel> GetIntakeLocationByLocationId(long intakeId, long locationId)
        {
            var result = await _intakeLocationRepository.SingleOrDefaultAsync(a => a.IntakeId == intakeId && a.LocationId == locationId);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "Service Type Email does not exists." };
            }
        }
    }
}
