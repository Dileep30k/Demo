using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Edu.Abstraction.Models;
using Edu.Repository.Interfaces;
using Edu.Service.Interfaces;
using Core.Utility.Utils;
using Microsoft.AspNetCore.Http;
using Edu.Abstraction.ComplexModels;

namespace Edu.Service.Services
{
    public class EmailAccountService : IEmailAccountService
    {
        private readonly IEmailAccountRepository _emailAccountRepository;
        private readonly IUserProviderService _userProviderService;

        public EmailAccountService(
            IUserProviderService userProviderService,
            IEmailAccountRepository emailAccountRepository)
        {
            _emailAccountRepository = emailAccountRepository;
            _userProviderService = userProviderService;
        }
        public async Task<ResponseModel> CreateEmailAccount(EmailAccount entity)
        {
            entity.CreatedBy = _userProviderService.UserClaim.UserId;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            await _emailAccountRepository.AddAsync(entity);
            var result = await _emailAccountRepository.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Email Account created successfully.",
                    Data = entity
                };
            }

            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Email Account does not created." };
        }
        public async Task<ResponseModel> DeleteEmailAccount(long id)
        {
            var entityResult = await GetEmailAccountById(id);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as EmailAccount;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            entity.IsDeleted = true;
            var result = await _emailAccountRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Email Account deleted successfully.",
                    Data = entity
                };
            }

            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Email Account does not deleted." };
        }
        public async Task<ResponseModel> GetAllEmailAccount(bool? isActive = null)
        {
            var currentDate = CommonUtils.GetDefaultDateTime();
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _emailAccountRepository.Find(a => a.IsDeleted == false && (!isActive.HasValue || a.IsActive == isActive))
            };
        }

        public async Task<ResponseModel> GetDefaultEmailAccount()
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _emailAccountRepository.SingleOrDefaultAsync(a => !a.IsDeleted && a.IsActive && a.IsDefaultAccount)
            };

        }

        public async Task<ResponseModel> GetEmailAccountById(long id)
        {
            var result = await _emailAccountRepository.GetByIdAsync(id);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "Email Account does not exists." };
            }
        }
        public async Task<ResponseModel> UpdateEmailAccount(EmailAccount updateEntity)
        {
            var entityResult = await GetEmailAccountById(updateEntity.EmailAccountId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as EmailAccount;

            entity.Email = updateEntity.Email;
            entity.DisplayName = updateEntity.DisplayName;
            entity.Host = updateEntity.Host;
            entity.Port = updateEntity.Port;
            entity.Username = updateEntity.Username;
            entity.Password = updateEntity.Password;
            entity.EnableSsl = updateEntity.EnableSsl;
            entity.UseDefaultCredentials = updateEntity.UseDefaultCredentials;
            entity.IsDefaultAccount = updateEntity.IsDefaultAccount;
            entity.IsActive = updateEntity.IsActive;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;

            var result = await _emailAccountRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Email Account updated successfully.",
                    Data = entity
                };
            }

            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Email Account does not updated." };
        }
    }
}
