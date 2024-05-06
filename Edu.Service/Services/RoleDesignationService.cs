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
using Edu.Abstraction.Enums;

namespace Edu.Service.Services
{
    public class RoleDesignationService : IRoleDesignationService
    {
        private readonly IRoleDesignationRepository _roleDesignationRepository;
        private readonly IUserProviderService _userProviderService;

        public RoleDesignationService(
            IRoleDesignationRepository roleDesignationRepository,
            IUserProviderService userProviderService
        )
        {
            _roleDesignationRepository = roleDesignationRepository;
            _userProviderService = userProviderService;
        }

        public async Task<ResponseModel> CreateRoleDesignation(RoleDesignation entity)
        {
            var existEntity = await GetRoleDesignationByDesignationId(entity.RoleId, entity.DesignationId);
            if (existEntity.Success)
            {
                return new ResponseModel
                {
                    Success = false,
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = "Role Designation already exists.",
                };
            }

            entity.CreatedBy = _userProviderService.UserClaim.UserId;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            CommonUtils.EncodeProperties(entity);
            await _roleDesignationRepository.AddAsync(entity);
            var result = await _roleDesignationRepository.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Role Designation created successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Role Designation does not created." };
        }

        public async Task<ResponseModel> DeleteRoleDesignation(long id)
        {
            var entityResult = await GetRoleDesignationById(id);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as RoleDesignation;
            _roleDesignationRepository.Remove(entity);
            var result = await _roleDesignationRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Role Designation deleted successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Role Designation does not deleted." };
        }

        public async Task<ResponseModel> GetAllRoleDesignation(bool? isActive = null)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _roleDesignationRepository.Find(a => a.IsDeleted == false && (!isActive.HasValue || a.IsActive == isActive))
            };
        }

        public async Task<ResponseModel> GetRoleDesignationPaged(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _roleDesignationRepository.GetRoleDesignationPaged(pagination)
            };
        }

        public async Task<List<long>> GetRolesByDesignationId(long? designationId)
        {
            var roles = new List<long>();
            if (designationId != null)
            {
                roles.AddRange(await _roleDesignationRepository.GetRolesByDesignationId(designationId.Value));
            }

            var employeeRole = Roles.Employee.GetHashCode();
            if (!roles.Any(r => r == employeeRole))
            {
                roles.Add(employeeRole);
            }

            return roles;
        }

        public async Task<ResponseModel> GetRoleDesignationById(long id)
        {
            var result = await _roleDesignationRepository.SingleOrDefaultAsync(a => a.RoleDesignationId == id);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "Role Designation does not exists." };
            }
        }

        public async Task<ResponseModel> UpdateRoleDesignation(RoleDesignation updateEntity)
        {
            var entityResult = await GetRoleDesignationById(updateEntity.RoleDesignationId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as RoleDesignation;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;

            CommonUtils.EncodeProperties(entity);
            var result = await _roleDesignationRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Role Designation updated successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Role Designation does not updated." };
        }

        public async Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null)
        {
            return (await _roleDesignationRepository.GetDropdwon(id, isActive)).Data;
        }

        private async Task<ResponseModel> GetRoleDesignationByDesignationId(long roleId, long designationId)
        {
            var result = await _roleDesignationRepository.SingleOrDefaultAsync(a => a.RoleId == roleId && a.DesignationId == designationId);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "Role Designation does not exists." };
            }
        }
    }
}
