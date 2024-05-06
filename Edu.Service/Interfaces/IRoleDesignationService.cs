using Core.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edu.Abstraction.ComplexModels;
using Edu.Abstraction.Models;

namespace Edu.Service.Interfaces
{
    public interface IRoleDesignationService
    {
        Task<ResponseModel> GetRoleDesignationById(long id);
        Task<ResponseModel> GetAllRoleDesignation(bool? isActive = null);
        Task<ResponseModel> GetRoleDesignationPaged(Pagination pagination);
        Task<ResponseModel> CreateRoleDesignation(RoleDesignation entity);
        Task<ResponseModel> UpdateRoleDesignation(RoleDesignation updateEntity);
        Task<ResponseModel> DeleteRoleDesignation(long id);
        Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null);
        Task<List<long>> GetRolesByDesignationId(long? designationId);
    }
}
