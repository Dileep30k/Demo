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
    public class SchemeInstituteService : ISchemeInstituteService
    {
        private readonly ISchemeInstituteRepository _schemeInstituteRepository;
        private readonly IUserProviderService _userProviderService;

        public SchemeInstituteService(
            ISchemeInstituteRepository schemeInstituteRepository,
            IUserProviderService userProviderService
        )
        {
            _schemeInstituteRepository = schemeInstituteRepository;
            _userProviderService = userProviderService;
        }

        public async Task<ResponseModel> CreateSchemeInstitute(SchemeInstitute entity)
        {
            var existEntity = await GetSchemeInstituteByInstituteId(entity.SchemeId, entity.InstituteId);
            if (existEntity.Success)
            {
                return new ResponseModel
                {
                    Success = false,
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = "Scheme Institute already exists.",
                };
            }

            entity.CreatedBy = _userProviderService.UserClaim.UserId;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            CommonUtils.EncodeProperties(entity);
            await _schemeInstituteRepository.AddAsync(entity);
            var result = await _schemeInstituteRepository.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Scheme Institute created successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Scheme Institute does not created." };
        }

        public async Task<ResponseModel> DeleteSchemeInstitute(long id)
        {
            var entityResult = await GetSchemeInstituteById(id);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as SchemeInstitute;
            _schemeInstituteRepository.Remove(entity);
            var result = await _schemeInstituteRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Scheme Institute deleted successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Scheme Institute does not deleted." };
        }

        public async Task<ResponseModel> GetAllSchemeInstitute(bool? isActive = null)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _schemeInstituteRepository.Find(a => a.IsDeleted == false && (!isActive.HasValue || a.IsActive == isActive))
            };
        }

        public async Task<ResponseModel> GetSchemeInstitutePaged(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _schemeInstituteRepository.GetSchemeInstitutePaged(pagination)
            };
        }

        public async Task<ResponseModel> GetSchemeInstituteById(long id)
        {
            var result = await _schemeInstituteRepository.SingleOrDefaultAsync(a => a.SchemeInstituteId == id);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "Scheme Institute does not exists." };
            }
        }

        public async Task<ResponseModel> UpdateSchemeInstitute(SchemeInstitute updateEntity)
        {
            var entityResult = await GetSchemeInstituteById(updateEntity.SchemeInstituteId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as SchemeInstitute;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;

            CommonUtils.EncodeProperties(entity);
            var result = await _schemeInstituteRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Scheme Institute updated successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Scheme Institute does not updated." };
        }

        public async Task<List<DropdownModel>> GetDropdwon(long? id = null, long? schemeId = null)
        {
            return (await _schemeInstituteRepository.GetDropdwon(id, schemeId)).Data;
        }

        private async Task<ResponseModel> GetSchemeInstituteByInstituteId(long schemeId, long instituteId)
        {
            var result = await _schemeInstituteRepository.SingleOrDefaultAsync(a => a.SchemeId == schemeId && a.InstituteId == instituteId);
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
