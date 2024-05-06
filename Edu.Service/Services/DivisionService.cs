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
    public class DivisionService : IDivisionService
    {
        private readonly IDivisionRepository _divisionRepository;
        private readonly IUserProviderService _userProviderService;

        public DivisionService(
            IDivisionRepository divisionRepository,
            IUserProviderService userProviderService
        )
        {
            _divisionRepository = divisionRepository;
            _userProviderService = userProviderService;
        }

        public async Task<ResponseModel> CreateDivision(Division entity)
        {
            var existEntity = await GetDivisionById(entity.DivisionId);
            if (existEntity.Success)
            {
                return new ResponseModel
                {
                    Success = false,
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = "Division already exists.",
                };
            }

            entity.CreatedBy = _userProviderService.UserClaim.UserId;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            CommonUtils.EncodeProperties(entity);
            await _divisionRepository.AddAsync(entity);
            var result = await _divisionRepository.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Division created successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Division does not created." };
        }

        public async Task<ResponseModel> DeleteDivision(long id)
        {
            var entityResult = await GetDivisionById(id);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as Division;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            entity.IsDeleted = true;
            var result = await _divisionRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Division deleted successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Division does not deleted." };
        }

        public async Task<ResponseModel> GetAllDivision(bool? isActive = null)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _divisionRepository.Find(a => a.IsDeleted == false && (!isActive.HasValue || a.IsActive == isActive))
            };
        }

        public async Task<ResponseModel> GetDivisionPaged(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _divisionRepository.GetDivisionPaged(pagination)
            };
        }

        public async Task<ResponseModel> GetDivisionById(long id)
        {
            var result = await _divisionRepository.SingleOrDefaultAsync(a => a.DivisionId == id);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "Division does not exists." };
            }
        }

        public async Task<ResponseModel> UpdateDivision(Division updateEntity)
        {
            var entityResult = await GetDivisionById(updateEntity.DivisionId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as Division;
            entity.DivisionName = updateEntity.DivisionName;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;

            CommonUtils.EncodeProperties(entity);
            var result = await _divisionRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Division updated successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Division does not updated." };
        }

        public async Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null)
        {
            return (await _divisionRepository.GetDropdwon(id, isActive)).Data;
        }
    }
}
