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
    public class DocumentTypeService : IDocumentTypeService
    {
        private readonly IDocumentTypeRepository _documentTypeRepository;
        private readonly IUserProviderService _userProviderService;

        public DocumentTypeService(
            IDocumentTypeRepository documentTypeRepository,
            IUserProviderService userProviderService
        )
        {
            _documentTypeRepository = documentTypeRepository;
            _userProviderService = userProviderService;
        }

        public async Task<ResponseModel> CreateDocumentType(DocumentType entity)
        {
            var existEntity = await GetDocumentTypeById(entity.DocumentTypeId);
            if (existEntity.Success)
            {
                return new ResponseModel
                {
                    Success = false,
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = "DocumentType already exists.",
                };
            }

            entity.CreatedBy = _userProviderService.UserClaim.UserId;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            CommonUtils.EncodeProperties(entity);
            await _documentTypeRepository.AddAsync(entity);
            var result = await _documentTypeRepository.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "DocumentType created successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "DocumentType does not created." };
        }

        public async Task<ResponseModel> DeleteDocumentType(long id)
        {
            var entityResult = await GetDocumentTypeById(id);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as DocumentType;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            entity.IsDeleted = true;
            var result = await _documentTypeRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "DocumentType deleted successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "DocumentType does not deleted." };
        }

        public async Task<ResponseModel> GetAllDocumentType(bool? isActive = null)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _documentTypeRepository.Find(a => a.IsDeleted == false && (!isActive.HasValue || a.IsActive == isActive))
            };
        }

        public async Task<ResponseModel> GetDocumentTypePaged(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _documentTypeRepository.GetDocumentTypePaged(pagination)
            };
        }

        public async Task<ResponseModel> GetDocumentTypeById(long id)
        {
            var result = await _documentTypeRepository.SingleOrDefaultAsync(a => a.DocumentTypeId == id);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "DocumentType does not exists." };
            }
        }

        public async Task<ResponseModel> UpdateDocumentType(DocumentType updateEntity)
        {
            var entityResult = await GetDocumentTypeById(updateEntity.DocumentTypeId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as DocumentType;
            entity.DocumentTypeName = updateEntity.DocumentTypeName;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;

            CommonUtils.EncodeProperties(entity);
            var result = await _documentTypeRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "DocumentType updated successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "DocumentType does not updated." };
        }

        public async Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null)
        {
            return (await _documentTypeRepository.GetDropdwon(id, isActive)).Data;
        }
    }
}
