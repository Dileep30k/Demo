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
    public interface IAdmissionStatusService
    {
        Task<ResponseModel> GetAdmissionStatusById(long id);
        Task<ResponseModel> GetAllAdmissionStatus(bool? isActive = null);
        Task<ResponseModel> GetAdmissionStatusPaged(Pagination pagination);
        Task<ResponseModel> CreateAdmissionStatus(AdmissionStatus entity);
        Task<ResponseModel> UpdateAdmissionStatus(AdmissionStatus updateEntity);
        Task<ResponseModel> DeleteAdmissionStatus(long id);
        Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null);
    }
}
