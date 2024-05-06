using Core.Repository.Models;
using Edu.Abstraction.ComplexModels;
using Edu.Abstraction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edu.Service.Interfaces
{
    public interface ISettingService
    {
        Task<ResponseModel> GetSettingById(long id);
        Task<ResponseModel> GetAllSetting(bool? isActive = null);
        Task<ResponseModel> GetSettingPaged(Pagination pagination);
        Task<ResponseModel> CreateSetting(Setting entity);
        Task<ResponseModel> UpdateSetting(Setting updateEntity);
        Task<ResponseModel> UpdateSettings(List<Setting> updateEntities);
        Task<ResponseModel> DeleteSetting(long id);
        Task<List<Setting>> GetSettingsByKeys(List<string> keys);
    }
}
