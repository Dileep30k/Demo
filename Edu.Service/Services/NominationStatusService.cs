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
    public class NominationStatusService : INominationStatusService
    {
        private readonly INominationStatusRepository _nominationStatusRepository;
        private readonly IUserProviderService _userProviderService;

        public NominationStatusService(
            INominationStatusRepository nominationStatusRepository,
            IUserProviderService userProviderService
        )
        {
            _nominationStatusRepository = nominationStatusRepository;
            _userProviderService = userProviderService;
        }

        public async Task<ResponseModel> CreateNominationStatus(NominationStatus entity)
        {
            var existEntity = await GetNominationStatusById(entity.NominationStatusId);
            if (existEntity.Success)
            {
                return new ResponseModel
                {
                    Success = false,
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = "NominationStatus already exists.",
                };
            }

            entity.CreatedBy = _userProviderService.UserClaim.UserId;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            CommonUtils.EncodeProperties(entity);
            await _nominationStatusRepository.AddAsync(entity);
            var result = await _nominationStatusRepository.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "NominationStatus created successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "NominationStatus does not created." };
        }

        public async Task<ResponseModel> DeleteNominationStatus(long id)
        {
            var entityResult = await GetNominationStatusById(id);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as NominationStatus;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            entity.IsDeleted = true;
            var result = await _nominationStatusRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "NominationStatus deleted successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "NominationStatus does not deleted." };
        }

        public async Task<ResponseModel> GetAllNominationStatus(bool? isActive = null)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _nominationStatusRepository.Find(a => a.IsDeleted == false && (!isActive.HasValue || a.IsActive == isActive))
            };
        }

        public async Task<ResponseModel> GetNominationStatusPaged(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _nominationStatusRepository.GetNominationStatusPaged(pagination)
            };
        }

        public async Task<ResponseModel> GetNominationStatusById(long id)
        {
            var result = await _nominationStatusRepository.SingleOrDefaultAsync(a => a.NominationStatusId == id);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "NominationStatus does not exists." };
            }
        }

        public async Task<ResponseModel> UpdateNominationStatus(NominationStatus updateEntity)
        {
            var entityResult = await GetNominationStatusById(updateEntity.NominationStatusId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as NominationStatus;
            entity.NominationStatusName = updateEntity.NominationStatusName;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;

            CommonUtils.EncodeProperties(entity);
            var result = await _nominationStatusRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "NominationStatus updated successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "NominationStatus does not updated." };
        }

        public async Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null)
        {
            return (await _nominationStatusRepository.GetDropdwon(id, isActive)).Data;
        }
    }
}
