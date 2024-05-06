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
    public class IntakeTemplateService : IIntakeTemplateService
    {
        private readonly IIntakeTemplateRepository _intakeTemplateRepository;
        private readonly IUserProviderService _userProviderService;

        public IntakeTemplateService(
            IIntakeTemplateRepository intakeTemplateRepository,
            IUserProviderService userProviderService
        )
        {
            _intakeTemplateRepository = intakeTemplateRepository;
            _userProviderService = userProviderService;
        }

        public async Task<ResponseModel> CreateIntakeTemplate(IntakeTemplate entity)
        {
            var existEntity = await GetIntakeTemplateByTemplateId(entity.IntakeId, entity.TemplateId);
            if (existEntity.Success)
            {
                return new ResponseModel
                {
                    Success = false,
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = "Intake Template already exists.",
                };
            }

            entity.CreatedBy = _userProviderService.UserClaim.UserId;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            CommonUtils.EncodeProperties(entity);
            await _intakeTemplateRepository.AddAsync(entity);
            var result = await _intakeTemplateRepository.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Intake Template created successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Intake Template does not created." };
        }

        public async Task<ResponseModel> DeleteIntakeTemplate(long id)
        {
            var entityResult = await GetIntakeTemplateById(id);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as IntakeTemplate;
            _intakeTemplateRepository.Remove(entity);
            var result = await _intakeTemplateRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Intake Template deleted successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Intake Template does not deleted." };
        }

        public async Task<ResponseModel> GetAllIntakeTemplate(long? intakeId = null)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _intakeTemplateRepository.Find(a => a.IsDeleted == false && (!intakeId.HasValue || a.IntakeId == intakeId))
            };
        }

        public async Task<ResponseModel> GetIntakeTemplatePaged(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _intakeTemplateRepository.GetIntakeTemplatePaged(pagination)
            };
        }

        public async Task<ResponseModel> GetIntakeTemplateById(long id)
        {
            var result = await _intakeTemplateRepository.SingleOrDefaultAsync(a => a.IntakeTemplateId == id);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "Intake Template does not exists." };
            }
        }

        public async Task<IntakeTemplate> GetIntakeTemplateByKey(long intakeId, string key)
        {
            return await _intakeTemplateRepository.GetIntakeTemplateByKey(intakeId, key);
        }

        public async Task<IList<IntakeTemplate>> GetIntakeTemplatesByKey(List<long> intakeIds, string key)
        {
            return await _intakeTemplateRepository.GetIntakeTemplatesByKey(intakeIds, key);
        }

        public async Task<ResponseModel> UpdateIntakeTemplate(IntakeTemplate updateEntity)
        {
            var entityResult = await GetIntakeTemplateById(updateEntity.IntakeTemplateId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as IntakeTemplate;
            entity.TemplateContent = updateEntity.TemplateContent;
            entity.TemplateSubject = updateEntity.TemplateSubject;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;

            var result = await _intakeTemplateRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Intake Template updated successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Intake Template does not updated." };
        }

        public async Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null)
        {
            return (await _intakeTemplateRepository.GetDropdwon(id, isActive)).Data;
        }

        private async Task<ResponseModel> GetIntakeTemplateByTemplateId(long intakeId, long templateId)
        {
            var result = await _intakeTemplateRepository.SingleOrDefaultAsync(a => a.IntakeId == intakeId && a.TemplateId == templateId);
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
