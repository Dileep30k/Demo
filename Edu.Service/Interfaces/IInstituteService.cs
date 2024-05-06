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
    public interface IInstituteService
    {
        Task<ResponseModel> GetInstituteById(long id);
        Task<ResponseModel> GetAllInstitute(bool? isActive = null);
        Task<ResponseModel> GetInstitutePaged(Pagination pagination);
        Task<ResponseModel> CreateInstitute(Institute entity);
        Task<ResponseModel> UpdateInstitute(Institute updateEntity);
        Task<ResponseModel> DeleteInstitute(long id);
        Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null);
    }
}
