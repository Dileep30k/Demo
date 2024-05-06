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
    public class IntakeInstituteService : IIntakeInstituteService
    {
        private readonly IIntakeInstituteRepository _intakeInstituteRepository;
        private readonly IUserProviderService _userProviderService;

        public IntakeInstituteService(
            IIntakeInstituteRepository intakeInstituteRepository,
            IUserProviderService userProviderService
        )
        {
            _intakeInstituteRepository = intakeInstituteRepository;
            _userProviderService = userProviderService;
        }

        public async Task<ResponseModel> CreateIntakeInstitute(IntakeInstitute entity)
        {
            var existEntity = await GetIntakeInstituteByInstituteId(entity.IntakeId, entity.InstituteId);
            if (existEntity.Success)
            {
                return new ResponseModel
                {
                    Success = false,
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = "Intake Institute already exists.",
                };
            }

            entity.CreatedBy = _userProviderService.UserClaim.UserId;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            CommonUtils.EncodeProperties(entity);
            await _intakeInstituteRepository.AddAsync(entity);
            var result = await _intakeInstituteRepository.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Intake Institute created successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Intake Institute does not created." };
        }

        public async Task<ResponseModel> DeleteIntakeInstitute(long id)
        {
            var entityResult = await GetIntakeInstituteById(id);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as IntakeInstitute;
            _intakeInstituteRepository.Remove(entity);
            var result = await _intakeInstituteRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Intake Institute deleted successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Intake Institute does not deleted." };
        }

        public async Task<ResponseModel> GetAllIntakeInstitute(bool? isActive = null)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _intakeInstituteRepository.Find(a => a.IsDeleted == false && (!isActive.HasValue || a.IsActive == isActive))
            };
        }

        public async Task<ResponseModel> GetIntakeInstitutePaged(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _intakeInstituteRepository.GetIntakeInstitutePaged(_userProviderService.UserClaim, pagination)
            };
        }

        public async Task<ResponseModel> GetIntakeInstituteById(long id)
        {
            var result = await _intakeInstituteRepository.SingleOrDefaultAsync(a => a.IntakeInstituteId == id);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "Intake Institute does not exists." };
            }
        }

        public async Task<ResponseModel> UpdateIntakeInstitute(IntakeInstitute updateEntity)
        {
            var entityResult = await GetIntakeInstituteById(updateEntity.IntakeInstituteId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as IntakeInstitute;
            entity.IntakeId = updateEntity.IntakeId;
            entity.InstituteId = updateEntity.InstituteId;
            entity.TotalSeats = updateEntity.TotalSeats;
            entity.AdmissionCutoffDate = updateEntity.AdmissionCutoffDate;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;

            CommonUtils.EncodeProperties(entity);
            var result = await _intakeInstituteRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Intake Institute updated successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Intake Institute does not updated." };
        }

        public async Task<List<DropdownModel>> GetDropdwon(long? id = null, long? intakeId = null)
        {
            return (await _intakeInstituteRepository.GetDropdwon(id, intakeId)).Data;
        }

        private async Task<ResponseModel> GetIntakeInstituteByInstituteId(long intakeId, long instituteId)
        {
            var result = await _intakeInstituteRepository.SingleOrDefaultAsync(a => a.IntakeId == intakeId && a.InstituteId == instituteId);
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
