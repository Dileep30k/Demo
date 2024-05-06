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
    public interface INominationStatusService
    {
        Task<ResponseModel> GetNominationStatusById(long id);
        Task<ResponseModel> GetAllNominationStatus(bool? isActive = null);
        Task<ResponseModel> GetNominationStatusPaged(Pagination pagination);
        Task<ResponseModel> CreateNominationStatus(NominationStatus entity);
        Task<ResponseModel> UpdateNominationStatus(NominationStatus updateEntity);
        Task<ResponseModel> DeleteNominationStatus(long id);
        Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null);
    }
}
