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
    public interface IBatchService
    {
        Task<ResponseModel> GetBatchById(long id);
        Task<ResponseModel> GetAllBatch(bool? isActive = null);
        Task<ResponseModel> GetBatchPaged(Pagination pagination);
        Task<ResponseModel> CreateBatch(Batch entity);
        Task<ResponseModel> UpdateBatch(Batch updateEntity);
        Task<ResponseModel> DeleteBatch(long id);
        Task<List<DropdownModel>> GetDropdwon(long? id = null, long? schemeId = null, long? intakeId = null, long? instituteId = null);
    }
}
