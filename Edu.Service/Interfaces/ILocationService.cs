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
    public interface ILocationService
    {
        Task<ResponseModel> GetLocationById(long id);
        Task<ResponseModel> GetAllLocation(bool? isActive = null);
        Task<ResponseModel> GetLocationPaged(Pagination pagination);
        Task<ResponseModel> CreateLocation(Location entity);
        Task<ResponseModel> UpdateLocation(Location updateEntity);
        Task<ResponseModel> DeleteLocation(long id);
        Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null);
    }
}
