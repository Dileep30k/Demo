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
    public interface ITemplateService
    {
        Task<ResponseModel> GetTemplateById(long id);
        Task<Template> GetTemplateByKey(string key);
        Task<ResponseModel> GetAllTemplate(bool? isActive = null);
        Task<ResponseModel> GetTemplatePaged(Pagination pagination);
        Task<ResponseModel> CreateTemplate(Template entity);
        Task<ResponseModel> UpdateTemplate(Template updateEntity);
        Task<ResponseModel> DeleteTemplate(long id);
        Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null);
    }
}
