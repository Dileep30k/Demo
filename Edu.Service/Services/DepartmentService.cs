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
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUserProviderService _userProviderService;

        public DepartmentService(
            IDepartmentRepository departmentRepository,
            IUserProviderService userProviderService
        )
        {
            _departmentRepository = departmentRepository;
            _userProviderService = userProviderService;
        }

        public async Task<ResponseModel> CreateDepartment(Department entity)
        {
            var existEntity = await GetDepartmentById(entity.DepartmentId);
            if (existEntity.Success)
            {
                return new ResponseModel
                {
                    Success = false,
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = "Department already exists.",
                };
            }

            entity.CreatedBy = _userProviderService.UserClaim.UserId;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            CommonUtils.EncodeProperties(entity);
            await _departmentRepository.AddAsync(entity);
            var result = await _departmentRepository.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Department created successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Department does not created." };
        }

        public async Task<ResponseModel> DeleteDepartment(long id)
        {
            var entityResult = await GetDepartmentById(id);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as Department;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            entity.IsDeleted = true;
            var result = await _departmentRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Department deleted successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Department does not deleted." };
        }

        public async Task<ResponseModel> GetAllDepartment(bool? isActive = null)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _departmentRepository.Find(a => a.IsDeleted == false && (!isActive.HasValue || a.IsActive == isActive))
            };
        }

        public async Task<ResponseModel> GetDepartmentPaged(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _departmentRepository.GetDepartmentPaged(pagination)
            };
        }

        public async Task<ResponseModel> GetDepartmentById(long id)
        {
            var result = await _departmentRepository.SingleOrDefaultAsync(a => a.DepartmentId == id);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "Department does not exists." };
            }
        }

        public async Task<ResponseModel> UpdateDepartment(Department updateEntity)
        {
            var entityResult = await GetDepartmentById(updateEntity.DepartmentId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as Department;
            entity.DepartmentName = updateEntity.DepartmentName;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;

            CommonUtils.EncodeProperties(entity);
            var result = await _departmentRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Department updated successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Department does not updated." };
        }

        public async Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null)
        {
            return (await _departmentRepository.GetDropdwon(id, isActive)).Data;
        }
    }
}
