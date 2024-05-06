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
    public interface ISchemeService
    {
        Task<ResponseModel> GetSchemeById(long id);
        Task<ResponseModel> GetAllScheme(List<long> schemeIds);
        Task<ResponseModel> GetSchemePaged(Pagination pagination);
        Task<ResponseModel> CreateScheme(Scheme entity);
        Task<ResponseModel> UpdateScheme(Scheme updateEntity);
        Task<ResponseModel> DeleteScheme(long id);
        Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null);
        Task<List<DropdownModel>> GetEmployeeDropdwon(long userId);
    }
}
