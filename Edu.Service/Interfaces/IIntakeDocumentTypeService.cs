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
    public interface IIntakeDocumentTypeService
    {
        Task<ResponseModel> GetIntakeDocumentTypeById(long id);
        Task<ResponseModel> GetAllIntakeDocumentType(bool? isActive = null);
        Task<ResponseModel> GetIntakeDocumentTypePaged(Pagination pagination);
        Task<ResponseModel> CreateIntakeDocumentType(IntakeDocumentType entity);
        Task<ResponseModel> UpdateIntakeDocumentType(IntakeDocumentType updateEntity);
        Task<ResponseModel> DeleteIntakeDocumentType(long id);
        Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null);
    }
}
