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
    public interface IIntakeInstituteService
    {
        Task<ResponseModel> GetIntakeInstituteById(long id);
        Task<ResponseModel> GetAllIntakeInstitute(bool? isActive = null);
        Task<ResponseModel> GetIntakeInstitutePaged(Pagination pagination);
        Task<ResponseModel> CreateIntakeInstitute(IntakeInstitute entity);
        Task<ResponseModel> UpdateIntakeInstitute(IntakeInstitute updateEntity);
        Task<ResponseModel> DeleteIntakeInstitute(long id);
        Task<List<DropdownModel>> GetDropdwon(long? id = null, long? intakeId = null);
    }
}
