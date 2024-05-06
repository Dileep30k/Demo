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
    public class InstituteService : IInstituteService
    {
        private readonly IInstituteRepository _instituteRepository;
        private readonly IUserProviderService _userProviderService;

        public InstituteService(
            IInstituteRepository instituteRepository,
            IUserProviderService userProviderService
        )
        {
            _instituteRepository = instituteRepository;
            _userProviderService = userProviderService;
        }

        public async Task<ResponseModel> CreateInstitute(Institute entity)
        {
            var existEntity = await GetInstituteByNameCode(entity.InstituteName, entity.InstituteCode);
            if (existEntity.Success)
            {
                return new ResponseModel
                {
                    Success = false,
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = "Institute Name/Code already exists.",
                };
            }

            entity.CreatedBy = _userProviderService.UserClaim.UserId;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            CommonUtils.EncodeProperties(entity);
            await _instituteRepository.AddAsync(entity);
            var result = await _instituteRepository.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Institute created successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Institute does not created." };
        }

        public async Task<ResponseModel> DeleteInstitute(long id)
        {
            var entityResult = await GetInstituteById(id);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as Institute;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            entity.IsDeleted = true;
            var result = await _instituteRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Institute deleted successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Institute does not deleted." };
        }

        public async Task<ResponseModel> GetAllInstitute(bool? isActive = null)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _instituteRepository.Find(a => a.IsDeleted == false && (!isActive.HasValue || a.IsActive == isActive))
            };
        }

        public async Task<ResponseModel> GetInstitutePaged(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _instituteRepository.GetInstitutePaged(pagination)
            };
        }

        public async Task<ResponseModel> GetInstituteById(long id)
        {
            var result = await _instituteRepository.SingleOrDefaultAsync(a => a.InstituteId == id);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "Institute does not exists." };
            }
        }

        public async Task<ResponseModel> UpdateInstitute(Institute updateEntity)
        {
            var entityResult = await GetInstituteById(updateEntity.InstituteId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as Institute;
            entity.InstituteName = updateEntity.InstituteName;
            entity.InstituteCode = updateEntity.InstituteCode;
            entity.EmailAddress = updateEntity.EmailAddress;
            entity.ContactNo = updateEntity.ContactNo;
            entity.ContactPerson = updateEntity.ContactPerson;
            entity.Address = updateEntity.Address;
            entity.Pincode = updateEntity.Pincode;
            entity.City = updateEntity.City;
            entity.State = updateEntity.State;
            entity.Country = updateEntity.Country;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;

            CommonUtils.EncodeProperties(entity);
            var result = await _instituteRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Institute updated successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Institute does not updated." };
        }

        public async Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null)
        {
            return (await _instituteRepository.GetDropdwon(id, isActive)).Data;
        }
        private async Task<ResponseModel> GetInstituteByNameCode(string name, string code)
        {
            var result = await _instituteRepository.SingleOrDefaultAsync(a => a.InstituteName == name || a.InstituteCode == code);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "Institute does not exists." };
            }
        }
    }
}
