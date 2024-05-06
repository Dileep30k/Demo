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
    public interface IVerticalService
    {
        Task<ResponseModel> GetVerticalById(long id);
        Task<ResponseModel> GetAllVertical(bool? isActive = null);
        Task<ResponseModel> GetVerticalPaged(Pagination pagination);
        Task<ResponseModel> CreateVertical(Vertical entity);
        Task<ResponseModel> UpdateVertical(Vertical updateEntity);
        Task<ResponseModel> DeleteVertical(long id);
        Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null);
    }
}
