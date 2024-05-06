using Edu.Abstraction.ComplexModels;
using Edu.Abstraction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edu.Service.Interfaces
{
    public interface IEmailAccountService
    {
        Task<ResponseModel> GetDefaultEmailAccount();
        Task<ResponseModel> GetEmailAccountById(long id);
        Task<ResponseModel> GetAllEmailAccount(bool? isActive = null);
        Task<ResponseModel> CreateEmailAccount(EmailAccount entity);
        Task<ResponseModel> UpdateEmailAccount(EmailAccount updateEntity);
        Task<ResponseModel> DeleteEmailAccount(long id);

    }
}
