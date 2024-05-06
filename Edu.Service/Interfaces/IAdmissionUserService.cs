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
    public interface IAdmissionUserService
    {
        Task<ResponseModel> GetAdmissionUserById(long id);
        Task<ResponseModel> GetAllAdmissionUser(bool? isActive = null);
        Task<ResponseModel> GetAdmissionUserPaged(Pagination pagination);
        Task<ResponseModel> CreateAdmissionUser(AdmissionUser entity);
        Task<ResponseModel> CreateAdmissionUsers(Admission admission, ICollection<AdmissionUserModel> entities);
        Task<ResponseModel> UpdateAdmissionUser(AdmissionUser updateEntity);
        Task<ResponseModel> DeleteAdmissionUser(long id);
        Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null);
        Task<IEnumerable<AdmissionUser>> GetAdmissionUserByNominationId(long id);
    }
}
