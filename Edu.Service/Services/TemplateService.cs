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
    public class TemplateService : ITemplateService
    {
        private readonly ITemplateRepository _templateRepository;
        private readonly IUserProviderService _userProviderService;

        public TemplateService(
            ITemplateRepository templateRepository,
            IUserProviderService userProviderService
        )
        {
            _templateRepository = templateRepository;
            _userProviderService = userProviderService;
        }

        public async Task<ResponseModel> CreateTemplate(Template entity)
        {
            entity.CreatedBy = _userProviderService.UserClaim.UserId;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            //CommonUtils.EncodeProperties(entity);
            await _templateRepository.AddAsync(entity);
            var result = await _templateRepository.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Template created successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Template does not created." };
        }

        public async Task<ResponseModel> DeleteTemplate(long id)
        {
            var entityResult = await GetTemplateById(id);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as Template;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            entity.IsDeleted = true;
            var result = await _templateRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Template deleted successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Template does not deleted." };
        }

        public async Task<ResponseModel> GetAllTemplate(bool? isActive = null)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _templateRepository.Find(a => a.IsDeleted == false && (!isActive.HasValue || a.IsActive == isActive))
            };
        }

        public async Task<ResponseModel> GetTemplatePaged(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _templateRepository.GetTemplatePaged(pagination)
            };
        }

        public async Task<ResponseModel> GetTemplateById(long id)
        {
            var result = await _templateRepository.SingleOrDefaultAsync(a => a.TemplateId == id);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "Template does not exists." };
            }
        }

        public async Task<Template> GetTemplateByKey(string key)
        {
            return await _templateRepository.SingleOrDefaultAsync(a => a.TemplateKey == key);
        }

        public async Task<ResponseModel> UpdateTemplate(Template updateEntity)
        {
            var entityResult = await GetTemplateById(updateEntity.TemplateId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as Template;
            entity.TemplateContent = updateEntity.TemplateContent;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;

            //CommonUtils.EncodeProperties(entity);
            var result = await _templateRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Template updated successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Template does not updated." };
        }

        public async Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null)
        {
            return (await _templateRepository.GetDropdwon(id, isActive)).Data;
        }
    }
}
