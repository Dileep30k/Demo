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
    public interface ISchemeInstituteService
    {
        Task<ResponseModel> GetSchemeInstituteById(long id);
        Task<ResponseModel> GetAllSchemeInstitute(bool? isActive = null);
        Task<ResponseModel> GetSchemeInstitutePaged(Pagination pagination);
        Task<ResponseModel> CreateSchemeInstitute(SchemeInstitute entity);
        Task<ResponseModel> UpdateSchemeInstitute(SchemeInstitute updateEntity);
        Task<ResponseModel> DeleteSchemeInstitute(long id);
        Task<List<DropdownModel>> GetDropdwon(long? id = null, long? schemeId = null);
    }
}
