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
    public class IntakeDocumentTypeService : IIntakeDocumentTypeService
    {
        private readonly IIntakeDocumentTypeRepository _intakeDocumentTypeRepository;
        private readonly IUserProviderService _userProviderService;

        public IntakeDocumentTypeService(
            IIntakeDocumentTypeRepository intakeDocumentTypeRepository,
            IUserProviderService userProviderService
        )
        {
            _intakeDocumentTypeRepository = intakeDocumentTypeRepository;
            _userProviderService = userProviderService;
        }

        public async Task<ResponseModel> CreateIntakeDocumentType(IntakeDocumentType entity)
        {
            var existEntity = await GetIntakeDocumentTypeByDocumentTypeId(entity.IntakeId, entity.DocumentTypeId);
            if (existEntity.Success)
            {
                return new ResponseModel
                {
                    Success = false,
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = "Intake Document Type already exists.",
                };
            }

            entity.CreatedBy = _userProviderService.UserClaim.UserId;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            CommonUtils.EncodeProperties(entity);
            await _intakeDocumentTypeRepository.AddAsync(entity);
            var result = await _intakeDocumentTypeRepository.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Intake Document Type created successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Intake Document Type does not created." };
        }

        public async Task<ResponseModel> DeleteIntakeDocumentType(long id)
        {
            var entityResult = await GetIntakeDocumentTypeById(id);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as IntakeDocumentType;
            _intakeDocumentTypeRepository.Remove(entity);
            var result = await _intakeDocumentTypeRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Intake Document Type deleted successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Intake Document Type does not deleted." };
        }

        public async Task<ResponseModel> GetAllIntakeDocumentType(bool? isActive = null)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _intakeDocumentTypeRepository.Find(a => a.IsDeleted == false && (!isActive.HasValue || a.IsActive == isActive))
            };
        }

        public async Task<ResponseModel> GetIntakeDocumentTypePaged(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _intakeDocumentTypeRepository.GetIntakeDocumentTypePaged(pagination)
            };
        }

        public async Task<ResponseModel> GetIntakeDocumentTypeById(long id)
        {
            var result = await _intakeDocumentTypeRepository.SingleOrDefaultAsync(a => a.IntakeDocumentTypeId == id);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "Intake Document Type does not exists." };
            }
        }

        public async Task<ResponseModel> UpdateIntakeDocumentType(IntakeDocumentType updateEntity)
        {
            var entityResult = await GetIntakeDocumentTypeById(updateEntity.IntakeDocumentTypeId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as IntakeDocumentType;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;

            CommonUtils.EncodeProperties(entity);
            var result = await _intakeDocumentTypeRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Intake Document Type updated successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Intake Document Type does not updated." };
        }

        public async Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null)
        {
            return (await _intakeDocumentTypeRepository.GetDropdwon(id, isActive)).Data;
        }

        private async Task<ResponseModel> GetIntakeDocumentTypeByDocumentTypeId(long intakeId, long documentTypeId)
        {
            var result = await _intakeDocumentTypeRepository.SingleOrDefaultAsync(a => a.IntakeId == intakeId && a.DocumentTypeId == documentTypeId);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "Intake Document Type does not exists." };
            }
        }
    }
}
