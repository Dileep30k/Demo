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
    public interface IDesignationService
    {
        Task<ResponseModel> GetDesignationById(long id);
        Task<ResponseModel> GetAllDesignation(bool? isActive = null);
        Task<ResponseModel> GetDesignationPaged(Pagination pagination);
        Task<ResponseModel> CreateDesignation(Designation entity);
        Task<ResponseModel> UpdateDesignation(Designation updateEntity);
        Task<ResponseModel> DeleteDesignation(long id);
        Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null);
    }
}
