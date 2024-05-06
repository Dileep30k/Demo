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
    public interface IIntakeTemplateService
    {
        Task<ResponseModel> GetIntakeTemplateById(long id);
        Task<ResponseModel> GetAllIntakeTemplate(long? intakeId = null);
        Task<ResponseModel> GetIntakeTemplatePaged(Pagination pagination);
        Task<ResponseModel> CreateIntakeTemplate(IntakeTemplate entity);
        Task<ResponseModel> UpdateIntakeTemplate(IntakeTemplate updateEntity);
        Task<ResponseModel> DeleteIntakeTemplate(long id);
        Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null);
        Task<IntakeTemplate> GetIntakeTemplateByKey(long intakeId, string key);
        Task<IList<IntakeTemplate>> GetIntakeTemplatesByKey(List<long> intakeIds, string key);
    }
}
