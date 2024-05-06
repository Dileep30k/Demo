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
    public class AdmissionUserService : IAdmissionUserService
    {
        private readonly IAdmissionUserRepository _admissionUserRepository;
        private readonly IUserProviderService _userProviderService;

        public AdmissionUserService(
            IAdmissionUserRepository admissionUserRepository,
            IUserProviderService userProviderService
        )
        {
            _admissionUserRepository = admissionUserRepository;
            _userProviderService = userProviderService;
        }

        public async Task<ResponseModel> CreateAdmissionUser(AdmissionUser entity)
        {
            var existEntity = await GetAdmissionUserById(entity.AdmissionUserId);
            if (existEntity.Success)
            {
                return new ResponseModel
                {
                    Success = false,
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = "Admission User already exists.",
                };
            }

            entity.CreatedBy = _userProviderService.UserClaim.UserId;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            CommonUtils.EncodeProperties(entity);
            await _admissionUserRepository.AddAsync(entity);
            var result = await _admissionUserRepository.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Admission User created successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Admission User does not created." };
        }

        public async Task<ResponseModel> CreateAdmissionUsers(Admission admission, ICollection<AdmissionUserModel> entities)
        {
            var result = 0;
            foreach (var batch in entities.Batch(100))
            {
                foreach (var u in batch)
                {
                    var entity = new AdmissionUser
                    {
                        AdmissionId = admission.AdmissionId,
                        UserId = u.UserId,
                        NominationId = u.NominationId,
                        NominationInstituteId = u.NominationInstituteId,
                        AdmissionStatusId = u.AdmissionStatusId,
                        Rank = u.Rank,
                        CreatedBy = _userProviderService.UserClaim.UserId,
                        UpdatedBy = _userProviderService.UserClaim.UserId
                    };
                    CommonUtils.EncodeProperties(entity);
                    await _admissionUserRepository.AddAsync(entity);
                }
                result = await _admissionUserRepository.SaveChangesAsync();
            }

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Admission created successfully.",
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Admission does not created." };
        }

        public async Task<ResponseModel> DeleteAdmissionUser(long id)
        {
            var entityResult = await GetAdmissionUserById(id);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as AdmissionUser;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            entity.IsDeleted = true;
            var result = await _admissionUserRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Admission User deleted successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Admission User does not deleted." };
        }

        public async Task<ResponseModel> GetAllAdmissionUser(bool? isActive = null)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _admissionUserRepository.Find(a => a.IsDeleted == false && (!isActive.HasValue || a.IsActive == isActive))
            };
        }

        public async Task<ResponseModel> GetAdmissionUserPaged(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _admissionUserRepository.GetAdmissionUserPaged(pagination)
            };
        }

        public async Task<ResponseModel> GetAdmissionUserById(long id)
        {
            var result = await _admissionUserRepository.SingleOrDefaultAsync(a => a.AdmissionUserId == id);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "Admission User does not exists." };
            }
        }

        public async Task<ResponseModel> UpdateAdmissionUser(AdmissionUser updateEntity)
        {
            var entityResult = await GetAdmissionUserById(updateEntity.AdmissionUserId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as AdmissionUser;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;

            CommonUtils.EncodeProperties(entity);
            var result = await _admissionUserRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Admission User updated successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Admission User does not updated." };
        }

        public async Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null)
        {
            return (await _admissionUserRepository.GetDropdwon(id, isActive)).Data;
        }

        public async Task<IEnumerable<AdmissionUser>> GetAdmissionUserByNominationId(long id)
        {
            return await _admissionUserRepository.Find(a => a.NominationId == id);
        }
    }
}
