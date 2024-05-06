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
    public class AdmissionStatusService : IAdmissionStatusService
    {
        private readonly IAdmissionStatusRepository _admissionStatusRepository;
        private readonly IUserProviderService _userProviderService;

        public AdmissionStatusService(
            IAdmissionStatusRepository admissionStatusRepository,
            IUserProviderService userProviderService
        )
        {
            _admissionStatusRepository = admissionStatusRepository;
            _userProviderService = userProviderService;
        }

        public async Task<ResponseModel> CreateAdmissionStatus(AdmissionStatus entity)
        {
            var existEntity = await GetAdmissionStatusById(entity.AdmissionStatusId);
            if (existEntity.Success)
            {
                return new ResponseModel
                {
                    Success = false,
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = "AdmissionStatus already exists.",
                };
            }

            entity.CreatedBy = _userProviderService.UserClaim.UserId;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            CommonUtils.EncodeProperties(entity);
            await _admissionStatusRepository.AddAsync(entity);
            var result = await _admissionStatusRepository.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "AdmissionStatus created successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "AdmissionStatus does not created." };
        }

        public async Task<ResponseModel> DeleteAdmissionStatus(long id)
        {
            var entityResult = await GetAdmissionStatusById(id);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as AdmissionStatus;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            entity.IsDeleted = true;
            var result = await _admissionStatusRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "AdmissionStatus deleted successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "AdmissionStatus does not deleted." };
        }

        public async Task<ResponseModel> GetAllAdmissionStatus(bool? isActive = null)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _admissionStatusRepository.Find(a => a.IsDeleted == false && (!isActive.HasValue || a.IsActive == isActive))
            };
        }

        public async Task<ResponseModel> GetAdmissionStatusPaged(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _admissionStatusRepository.GetAdmissionStatusPaged(pagination)
            };
        }

        public async Task<ResponseModel> GetAdmissionStatusById(long id)
        {
            var result = await _admissionStatusRepository.SingleOrDefaultAsync(a => a.AdmissionStatusId == id);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "AdmissionStatus does not exists." };
            }
        }

        public async Task<ResponseModel> UpdateAdmissionStatus(AdmissionStatus updateEntity)
        {
            var entityResult = await GetAdmissionStatusById(updateEntity.AdmissionStatusId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as AdmissionStatus;
            entity.AdmissionStatusName = updateEntity.AdmissionStatusName;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;

            CommonUtils.EncodeProperties(entity);
            var result = await _admissionStatusRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "AdmissionStatus updated successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "AdmissionStatus does not updated." };
        }

        public async Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null)
        {
            return (await _admissionStatusRepository.GetDropdwon(id, isActive)).Data;
        }
    }
}
