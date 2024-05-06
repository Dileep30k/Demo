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
    public class DesignationService : IDesignationService
    {
        private readonly IDesignationRepository _designationRepository;
        private readonly IUserProviderService _userProviderService;

        public DesignationService(
            IDesignationRepository designationRepository,
            IUserProviderService userProviderService
        )
        {
            _designationRepository = designationRepository;
            _userProviderService = userProviderService;
        }

        public async Task<ResponseModel> CreateDesignation(Designation entity)
        {
            var existEntity = await GetDesignationById(entity.DesignationId);
            if (existEntity.Success)
            {
                return new ResponseModel
                {
                    Success = false,
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = "Designation already exists.",
                };
            }

            entity.CreatedBy = _userProviderService.UserClaim.UserId;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            CommonUtils.EncodeProperties(entity);
            await _designationRepository.AddAsync(entity);
            var result = await _designationRepository.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Designation created successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Designation does not created." };
        }

        public async Task<ResponseModel> DeleteDesignation(long id)
        {
            var entityResult = await GetDesignationById(id);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as Designation;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            entity.IsDeleted = true;
            var result = await _designationRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Designation deleted successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Designation does not deleted." };
        }

        public async Task<ResponseModel> GetAllDesignation(bool? isActive = null)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _designationRepository.Find(a => a.IsDeleted == false && (!isActive.HasValue || a.IsActive == isActive))
            };
        }

        public async Task<ResponseModel> GetDesignationPaged(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _designationRepository.GetDesignationPaged(pagination)
            };
        }

        public async Task<ResponseModel> GetDesignationById(long id)
        {
            var result = await _designationRepository.SingleOrDefaultAsync(a => a.DesignationId == id);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "Designation does not exists." };
            }
        }

        public async Task<ResponseModel> UpdateDesignation(Designation updateEntity)
        {
            var entityResult = await GetDesignationById(updateEntity.DesignationId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as Designation;
            entity.DesignationName = updateEntity.DesignationName;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;

            CommonUtils.EncodeProperties(entity);
            var result = await _designationRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Designation updated successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Designation does not updated." };
        }

        public async Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null)
        {
            return (await _designationRepository.GetDropdwon(id, isActive)).Data;
        }
    }
}
