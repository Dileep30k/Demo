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
    public interface INominationInstituteService
    {
        Task<ResponseModel> GetNominationInstituteById(long id);
        Task<ResponseModel> GetAllNominationInstitute(bool? isActive = null);
        Task<ResponseModel> GetNominationInstitutePaged(Pagination pagination);
        Task<ResponseModel> CreateNominationInstitute(NominationInstitute entity);
        Task<ResponseModel> UpdateNominationInstitute(NominationInstitute updateEntity);
        Task<ResponseModel> DeleteNominationInstitute(long id);
        Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null);
    }
}
