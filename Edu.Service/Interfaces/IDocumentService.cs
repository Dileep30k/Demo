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
    public interface IDocumentService
    {
        Task<ResponseModel> GetDocumentById(long id);
        Task<ResponseModel> GetAllDocument(bool? isActive = null);
        Task<ResponseModel> GetDocumentPaged(Pagination pagination);
        Task<ResponseModel> CreateDocument(Document entity);
        Task<ResponseModel> CreateDocuments(List<Document> entities);
        Task<ResponseModel> UpdateDocument(Document updateEntity);
        Task<ResponseModel> DeleteDocument(long id);
        Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null);
    }
}
