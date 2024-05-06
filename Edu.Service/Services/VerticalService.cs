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
    public class VerticalService : IVerticalService
    {
        private readonly IVerticalRepository _verticalRepository;
        private readonly IUserProviderService _userProviderService;

        public VerticalService(
            IVerticalRepository verticalRepository,
            IUserProviderService userProviderService
        )
        {
            _verticalRepository = verticalRepository;
            _userProviderService = userProviderService;
        }

        public async Task<ResponseModel> CreateVertical(Vertical entity)
        {
            var existEntity = await GetVerticalById(entity.VerticalId);
            if (existEntity.Success)
            {
                return new ResponseModel
                {
                    Success = false,
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = "Vertical already exists.",
                };
            }

            entity.CreatedBy = _userProviderService.UserClaim.UserId;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            CommonUtils.EncodeProperties(entity);
            await _verticalRepository.AddAsync(entity);
            var result = await _verticalRepository.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Vertical created successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Vertical does not created." };
        }

        public async Task<ResponseModel> DeleteVertical(long id)
        {
            var entityResult = await GetVerticalById(id);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as Vertical;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            entity.IsDeleted = true;
            var result = await _verticalRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Vertical deleted successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Vertical does not deleted." };
        }

        public async Task<ResponseModel> GetAllVertical(bool? isActive = null)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _verticalRepository.Find(a => a.IsDeleted == false && (!isActive.HasValue || a.IsActive == isActive))
            };
        }

        public async Task<ResponseModel> GetVerticalPaged(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _verticalRepository.GetVerticalPaged(pagination)
            };
        }

        public async Task<ResponseModel> GetVerticalById(long id)
        {
            var result = await _verticalRepository.SingleOrDefaultAsync(a => a.VerticalId == id);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "Vertical does not exists." };
            }
        }

        public async Task<ResponseModel> UpdateVertical(Vertical updateEntity)
        {
            var entityResult = await GetVerticalById(updateEntity.VerticalId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as Vertical;
            entity.VerticalName = updateEntity.VerticalName;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;

            CommonUtils.EncodeProperties(entity);
            var result = await _verticalRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Vertical updated successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Vertical does not updated." };
        }

        public async Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null)
        {
            return (await _verticalRepository.GetDropdwon(id, isActive)).Data;
        }
    }
}
