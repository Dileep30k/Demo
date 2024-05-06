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
using Edu.Abstraction.Enums;
using Edu.Repository.Contexts;

namespace Edu.Service.Services
{
    public class BatchService : IBatchService
    {
        private readonly IBatchRepository _batchRepository;
        private readonly INominationService _nominationService;
        private readonly IBatchUserService _batchUserService;
        private readonly IUserProviderService _userProviderService;
        private readonly IIntakeService _intakeService;
        private readonly IIntakeTemplateService _intakeTemplateService;
        private readonly IInstituteService _instituteService;
        private readonly ISettingService _settingService;
        private readonly IEmailSender _emailSender;
        private readonly IUserService _userService;
        private readonly ISchemeService _schemeService;

        public BatchService(
            IBatchRepository batchRepository,
            INominationService nominationService,
            IBatchUserService batchUserService,
            IUserProviderService userProviderService,
            IIntakeService intakeService,
            IIntakeTemplateService intakeTemplateService,
            IInstituteService instituteService,
            ISettingService settingService,
            IEmailSender emailSender,
            IUserService userService,
            ISchemeService schemeService
        )
        {
            _batchRepository = batchRepository;
            _nominationService = nominationService;
            _batchUserService = batchUserService;
            _userProviderService = userProviderService;
            _intakeService = intakeService;
            _intakeTemplateService = intakeTemplateService;
            _instituteService = instituteService;
            _settingService = settingService;
            _emailSender = emailSender;
            _userService = userService;
            _schemeService = schemeService;
        }

        public async Task<ResponseModel> CreateBatch(Batch entity)
        {
            var batchCount = await _batchRepository.GetBatchCount(entity);
            entity.BatchCode = $"{batchCount + 1:D2}";
            entity.CreatedBy = _userProviderService.UserClaim.UserId;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            CommonUtils.EncodeProperties(entity);
            await _batchRepository.AddAsync(entity);
            var result = await _batchRepository.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Batch created successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Batch does not created." };
        }

        public async Task<ResponseModel> DeleteBatch(long id)
        {
            var entityResult = await GetBatchById(id);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as Batch;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            entity.IsDeleted = true;
            var result = await _batchRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Batch deleted successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Batch does not deleted." };
        }

        public async Task<ResponseModel> GetAllBatch(bool? isActive = null)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _batchRepository.Find(a => a.IsDeleted == false && (!isActive.HasValue || a.IsActive == isActive))
            };
        }

        public async Task<ResponseModel> GetBatchPaged(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _batchRepository.GetBatchPaged(pagination)
            };
        }

        public async Task<ResponseModel> GetBatchById(long id)
        {
            var result = await _batchRepository.SingleOrDefaultAsync(a => a.BatchId == id);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "Batch does not exists." };
            }
        }

        public async Task<ResponseModel> UpdateBatch(Batch updateEntity)
        {
            var entityResult = await GetBatchById(updateEntity.BatchId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as Batch;

            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;

            CommonUtils.EncodeProperties(entity);
            var result = await _batchRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Batch updated successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Batch does not updated." };
        }

        public async Task<List<DropdownModel>> GetDropdwon(long? id = null, long? schemeId = null, long? intakeId = null, long? instituteId = null)
        {
            return (await _batchRepository.GetDropdwon(id, schemeId, intakeId, instituteId)).Data;
        }
    }
}
