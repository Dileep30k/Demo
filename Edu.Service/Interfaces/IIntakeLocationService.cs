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
    public interface IIntakeLocationService
    {
        Task<ResponseModel> GetIntakeLocationById(long id);
        Task<ResponseModel> GetAllIntakeLocation(bool? isActive = null);
        Task<ResponseModel> GetIntakeLocationPaged(Pagination pagination);
        Task<ResponseModel> CreateIntakeLocation(IntakeLocation entity);
        Task<ResponseModel> UpdateIntakeLocation(IntakeLocation updateEntity);
        Task<ResponseModel> DeleteIntakeLocation(long id);
        Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null);
    }
}
