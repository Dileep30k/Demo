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
    public interface IIntakeService
    {
        Task<ResponseModel> GetIntakeById(long id);
        Task<ResponseModel> GetAllIntake(bool? isActive = null);
        Task<ResponseModel> GetIntakePaged(Pagination pagination);
        Task<ResponseModel> CreateIntake(Intake entity);
        Task<ResponseModel> UpdateIntake(Intake updateEntity);
        Task<ResponseModel> UpdateIntakeBrochure(Intake updateEntity);
        Task<ResponseModel> DeleteIntake(long id);
        Task<List<DropdownModel>> GetDropdwon(long? id = null, long? schemeId = null, bool? isActive = null);
        Task<List<DropdownModel>> GetEmployeeDropdwon(long schemeId, long userId);
        Task<IEnumerable<Intake>> GetAllScorecardIntake();
    }
}
