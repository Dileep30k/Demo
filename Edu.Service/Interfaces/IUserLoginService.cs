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
    public interface IUserLoginService
    {
        Task<ResponseModel> GetUserLoginById(long id);
        Task<ResponseModel> GetUserLoginByMsilUserId(long msilUserId);
        Task<ResponseModel> CreateUserLogin(UserLogin entity);
        Task<ResponseModel> UpdateUserLogin(UserLogin updateEntity);
        Task<ResponseModel> DeleteUserLoginByEmail();
    }
}
