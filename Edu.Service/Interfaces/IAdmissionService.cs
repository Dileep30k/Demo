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
    public interface IAdmissionService
    {
        Task<ResponseModel> GetAdmissionById(long id);
        Task<ResponseModel> GetAdmission(long schemeId, long intakeId, long instituteId, bool isGts);
        Task<ResponseModel> GetAllAdmission(bool? isActive = null);
        Task<ResponseModel> GetAdmissionPaged(Pagination pagination);
        Task<ResponseModel> CreateAdmission(Admission entity);
        Task<ResponseModel> UpdateAdmission(Admission updateEntity, bool isPublish);
        Task<ResponseModel> UploadAdmissions(ICollection<AdmissionUserModel> admissionUsers);
        Task<ResponseModel> ConfirmAdmissions(ICollection<AdmissionUserModel> admissionUsers);
        Task<ResponseModel> DeleteAdmission(long id);
        Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null);
        Task<ResponseModel> GetAdmissionInstitutes(Pagination pagination);
        Task<ResponseModel> GetAdmissionUsers(Pagination pagination);
        Task<ResponseModel> GetConfirmAdmissionUsers(Pagination pagination);
        Task<ResponseModel> UpdateAdmissionUser(AdmissionUser updateEntity);
        Task<ResponseModel> UpdateAdmissionUserStatus(AdmissionUser updateEntity);
        Task<ResponseModel> UpdateAdmissionUserLegal(AdmissionUser updateEntity);
        Task<int> GetTotalAdmission(dynamic filters);
        Task<int> GetTotalWaillist(dynamic filters);
        Task<int> GetPendingServiceAgreement(dynamic filters);
        Task<int> GetPendingAdmissionConfirmation(dynamic filters);
        Task<ResponseModel> GetAdmissionPagedByDesg(Pagination pagination);
        Task<ResponseModel> GetAdmissionPagedByDiv(Pagination pagination);
    }
}
