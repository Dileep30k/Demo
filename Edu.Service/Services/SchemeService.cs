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
    public class SchemeService : ISchemeService
    {
        private readonly ISchemeRepository _schemeRepository;
        private readonly IUserProviderService _userProviderService;

        public SchemeService(
            ISchemeRepository schemeRepository,
            IUserProviderService userProviderService
        )
        {
            _schemeRepository = schemeRepository;
            _userProviderService = userProviderService;
        }

        public async Task<ResponseModel> CreateScheme(Scheme entity)
        {
            var existEntity = await GetSchemeById(entity.SchemeId);
            if (existEntity.Success)
            {
                return new ResponseModel
                {
                    Success = false,
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = "Scheme already exists.",
                };
            }

            foreach (var instituteId in entity.SelectedInstitutes.Split(',').Select(long.Parse).ToList())
            {
                entity.Institutes.Add(new SchemeInstitute { InstituteId = instituteId });
            }

            entity.CreatedBy = _userProviderService.UserClaim.UserId;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            CommonUtils.EncodeProperties(entity);
            await _schemeRepository.AddAsync(entity);
            var result = await _schemeRepository.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Scheme created successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Scheme does not created." };
        }

        public async Task<ResponseModel> DeleteScheme(long id)
        {
            var entityResult = await GetSchemeById(id);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as Scheme;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            entity.IsDeleted = true;
            var result = await _schemeRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Scheme deleted successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Scheme does not deleted." };
        }

        public async Task<ResponseModel> GetAllScheme(List<long> schemeIds)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _schemeRepository.Find(a => a.IsDeleted == false && schemeIds.Contains(a.SchemeId))
            };
        }

        public async Task<ResponseModel> GetSchemePaged(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _schemeRepository.GetSchemePaged(pagination)
            };
        }

        public async Task<ResponseModel> GetSchemeById(long id)
        {
            var result = await _schemeRepository.SingleOrDefaultAsync(a => a.SchemeId == id, b => b.Institutes);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "Scheme does not exists." };
            }
        }

        public async Task<ResponseModel> UpdateScheme(Scheme updateEntity)
        {
            var entityResult = await GetSchemeById(updateEntity.SchemeId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as Scheme;
            entity.SchemeName = updateEntity.SchemeName;
            entity.SchemeCode = updateEntity.SchemeCode;
            entity.Duration = updateEntity.Duration;
            entity.DurationTypeId = updateEntity.DurationTypeId;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;

            var institutes = new List<SchemeInstitute>();
            foreach (var instituteId in updateEntity.SelectedInstitutes.Split(',').Select(long.Parse).ToList())
            {
                var institute = entity.Institutes.FirstOrDefault(ur => ur.InstituteId == instituteId);
                if (institute != null)
                {
                    institutes.Add(institute);
                }
                else
                {
                    institutes.Add(new SchemeInstitute { InstituteId = instituteId });
                }
            }

            entity.Institutes = institutes;
            CommonUtils.EncodeProperties(entity);
            var result = await _schemeRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Scheme updated successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Scheme does not updated." };
        }

        public async Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null)
        {
            return (await _schemeRepository.GetDropdwon(id, isActive)).Data;
        }


        public async Task<List<DropdownModel>> GetEmployeeDropdwon(long userId)
        {
            return (await _schemeRepository.GetEmployeeDropdwon(userId)).Data;
        }
    }
}
