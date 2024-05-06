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
    public class NominationInstituteService : INominationInstituteService
    {
        private readonly INominationInstituteRepository _nominationInstituteRepository;
        private readonly IUserProviderService _userProviderService;

        public NominationInstituteService(
            INominationInstituteRepository nominationInstituteRepository,
            IUserProviderService userProviderService
        )
        {
            _nominationInstituteRepository = nominationInstituteRepository;
            _userProviderService = userProviderService;
        }

        public async Task<ResponseModel> CreateNominationInstitute(NominationInstitute entity)
        {
            var existEntity = await GetNominationInstituteById(entity.NominationInstituteId);
            if (existEntity.Success)
            {
                return new ResponseModel
                {
                    Success = false,
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = "Nomination Institute already exists.",
                };
            }

            entity.CreatedBy = _userProviderService.UserClaim.UserId;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            CommonUtils.EncodeProperties(entity);
            await _nominationInstituteRepository.AddAsync(entity);
            var result = await _nominationInstituteRepository.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Nomination Institute created successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Nomination Institute does not created." };
        }

        public async Task<ResponseModel> DeleteNominationInstitute(long id)
        {
            var entityResult = await GetNominationInstituteById(id);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as NominationInstitute;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            entity.IsDeleted = true;
            var result = await _nominationInstituteRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Nomination Institute deleted successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Nomination Institute does not deleted." };
        }

        public async Task<ResponseModel> GetAllNominationInstitute(bool? isActive = null)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _nominationInstituteRepository.Find(a => a.IsDeleted == false && (!isActive.HasValue || a.IsActive == isActive))
            };
        }

        public async Task<ResponseModel> GetNominationInstitutePaged(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _nominationInstituteRepository.GetNominationInstitutePaged(pagination)
            };
        }

        public async Task<ResponseModel> GetNominationInstituteById(long id)
        {
            var result = await _nominationInstituteRepository.SingleOrDefaultAsync(a => a.NominationInstituteId == id);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "Nomination Institute does not exists." };
            }
        }

        public async Task<ResponseModel> UpdateNominationInstitute(NominationInstitute updateEntity)
        {
            var entityResult = await GetNominationInstituteById(updateEntity.NominationInstituteId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as NominationInstitute;
            entity.NominationId = updateEntity.NominationId;
            entity.InstituteId = updateEntity.InstituteId;
            entity.IsExamTaken = updateEntity.IsExamTaken;
            entity.Rank = updateEntity.Rank;
            entity.Score = updateEntity.Score;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;

            CommonUtils.EncodeProperties(entity);
            var result = await _nominationInstituteRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Nomination Institute updated successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Nomination Institute does not updated." };
        }

        public async Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null)
        {
            return (await _nominationInstituteRepository.GetDropdwon(id, isActive)).Data;
        }
    }
}
