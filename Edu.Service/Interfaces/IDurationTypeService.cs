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
    public interface IDurationTypeService
    {
        Task<ResponseModel> GetDurationTypeById(long id);
        Task<ResponseModel> GetAllDurationType(bool? isActive = null);
        Task<ResponseModel> GetDurationTypePaged(Pagination pagination);
        Task<ResponseModel> CreateDurationType(DurationType entity);
        Task<ResponseModel> UpdateDurationType(DurationType updateEntity);
        Task<ResponseModel> DeleteDurationType(long id);
        Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null);
    }
}
