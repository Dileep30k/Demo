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
    public interface IDepartmentService
    {
        Task<ResponseModel> GetDepartmentById(long id);
        Task<ResponseModel> GetAllDepartment(bool? isActive = null);
        Task<ResponseModel> GetDepartmentPaged(Pagination pagination);
        Task<ResponseModel> CreateDepartment(Department entity);
        Task<ResponseModel> UpdateDepartment(Department updateEntity);
        Task<ResponseModel> DeleteDepartment(long id);
        Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null);
    }
}
