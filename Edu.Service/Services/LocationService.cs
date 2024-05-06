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
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IUserProviderService _userProviderService;

        public LocationService(
            ILocationRepository locationRepository,
            IUserProviderService userProviderService
        )
        {
            _locationRepository = locationRepository;
            _userProviderService = userProviderService;
        }

        public async Task<ResponseModel> CreateLocation(Location entity)
        {
            var existEntity = await GetLocationById(entity.LocationId);
            if (existEntity.Success)
            {
                return new ResponseModel
                {
                    Success = false,
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = "Location already exists.",
                };
            }

            entity.CreatedBy = _userProviderService.UserClaim.UserId;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            CommonUtils.EncodeProperties(entity);
            await _locationRepository.AddAsync(entity);
            var result = await _locationRepository.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Location created successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Location does not created." };
        }

        public async Task<ResponseModel> DeleteLocation(long id)
        {
            var entityResult = await GetLocationById(id);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as Location;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            entity.IsDeleted = true;
            var result = await _locationRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Location deleted successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Location does not deleted." };
        }

        public async Task<ResponseModel> GetAllLocation(bool? isActive = null)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _locationRepository.Find(a => a.IsDeleted == false && (!isActive.HasValue || a.IsActive == isActive))
            };
        }

        public async Task<ResponseModel> GetLocationPaged(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _locationRepository.GetLocationPaged(pagination)
            };
        }

        public async Task<ResponseModel> GetLocationById(long id)
        {
            var result = await _locationRepository.SingleOrDefaultAsync(a => a.LocationId == id);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "Location does not exists." };
            }
        }

        public async Task<ResponseModel> UpdateLocation(Location updateEntity)
        {
            var entityResult = await GetLocationById(updateEntity.LocationId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as Location;
            entity.LocationName = updateEntity.LocationName;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;

            CommonUtils.EncodeProperties(entity);
            var result = await _locationRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Location updated successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Location does not updated." };
        }

        public async Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null)
        {
            return (await _locationRepository.GetDropdwon(id, isActive)).Data;
        }
    }
}
