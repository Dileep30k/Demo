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
    public interface IBatchUserService
    {
        Task<ResponseModel> GetBatchUserById(long id);
        Task<ResponseModel> GetAllBatchUser(bool? isActive = null);
        Task<ResponseModel> GetBatchUserPaged(Pagination pagination);
        Task<ResponseModel> CreateBatchUser(BatchUser entity);
        Task<ResponseModel> UpdateBatchUser(BatchUser updateEntity);
        Task<ResponseModel> DeleteBatchUser(long id);
        Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null);
    }
}
