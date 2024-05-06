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
    public interface IDivisionService
    {
        Task<ResponseModel> GetDivisionById(long id);
        Task<ResponseModel> GetAllDivision(bool? isActive = null);
        Task<ResponseModel> GetDivisionPaged(Pagination pagination);
        Task<ResponseModel> CreateDivision(Division entity);
        Task<ResponseModel> UpdateDivision(Division updateEntity);
        Task<ResponseModel> DeleteDivision(long id);
        Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null);
    }
}
