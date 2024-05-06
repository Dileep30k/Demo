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
    public interface IDocumentTypeService
    {
        Task<ResponseModel> GetDocumentTypeById(long id);
        Task<ResponseModel> GetAllDocumentType(bool? isActive = null);
        Task<ResponseModel> GetDocumentTypePaged(Pagination pagination);
        Task<ResponseModel> CreateDocumentType(DocumentType entity);
        Task<ResponseModel> UpdateDocumentType(DocumentType updateEntity);
        Task<ResponseModel> DeleteDocumentType(long id);
        Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null);
    }
}
