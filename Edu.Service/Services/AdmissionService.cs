using Core.Repository.Models;
using Core.Security;
using Core.Utility.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edu.Abstraction.ComplexModels;
using Edu.Abstraction.Models;
using Edu.Repository.Interfaces;
using Edu.Service.Interfaces;
using Edu.Abstraction.Enums;
using System.Dynamic;
using Edu.Repository.Repositories;

namespace Edu.Service.Services
{
    public class AdmissionService : IAdmissionService
    {
        private readonly IAdmissionRepository _admissionRepository;
        private readonly INominationService _nominationService;
        private readonly IAdmissionUserService _admissionUserService;
        private readonly IUserProviderService _userProviderService;
        private readonly IIntakeService _intakeService;
        private readonly IIntakeTemplateService _intakeTemplateService;
        private readonly IInstituteService _instituteService;
        private readonly ISettingService _settingService;
        private readonly IEmailSender _emailSender;
        private readonly IUserService _userService;
        private readonly ISchemeService _schemeService;
        private readonly IBatchService _batchService;

        public AdmissionService(
            IAdmissionRepository admissionRepository,
            INominationService nominationService,
            IAdmissionUserService admissionUserService,
            IUserProviderService userProviderService,
            IIntakeService intakeService,
            IIntakeTemplateService intakeTemplateService,
            IInstituteService instituteService,
            ISettingService settingService,
            IEmailSender emailSender,
            IUserService userService,
            ISchemeService schemeService,
            IBatchService batchService
        )
        {
            _admissionRepository = admissionRepository;
            _nominationService = nominationService;
            _admissionUserService = admissionUserService;
            _userProviderService = userProviderService;
            _intakeService = intakeService;
            _intakeTemplateService = intakeTemplateService;
            _instituteService = instituteService;
            _settingService = settingService;
            _emailSender = emailSender;
            _userService = userService;
            _schemeService = schemeService;
            _batchService = batchService;
        }

        public async Task<ResponseModel> CreateAdmission(Admission entity)
        {
            entity.CreatedBy = _userProviderService.UserClaim.UserId;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            CommonUtils.EncodeProperties(entity);
            await _admissionRepository.AddAsync(entity);
            var result = await _admissionRepository.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Admission created successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Admission does not created." };
        }

        public async Task<ResponseModel> UploadAdmissions(ICollection<AdmissionUserModel> admissionUsers)
        {
            if (!admissionUsers.Any())
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status204NoContent, Message = "No Admission uploaded" };
            }
            var firstUser = admissionUsers.FirstOrDefault();
            if (firstUser.AdmissionId > 0)
            {
                await DeleteAdmission(firstUser.AdmissionId);
            }

            var entity = new Admission
            {
                AdmissionId = 0,
                InstituteId = firstUser.InstituteId,
                IntakeId = firstUser.IntakeId,
                SchemeId = firstUser.SchemeId,
                ApprovalBy1 = firstUser.ApprovalBy1,
                ApprovalBy2 = firstUser.ApprovalBy2,
                CreatedBy = _userProviderService.UserClaim.UserId,
                UpdatedBy = _userProviderService.UserClaim.UserId
            };
            CommonUtils.EncodeProperties(entity);
            await _admissionRepository.AddAsync(entity);
            var result = await _admissionRepository.SaveChangesAsync();
            if (result > 0)
            {
                return await _admissionUserService.CreateAdmissionUsers(entity, admissionUsers);
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Admissions does not created." };
        }

        public async Task<ResponseModel> DeleteAdmission(long id)
        {
            var entityResult = await GetAdmissionById(id);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as Admission;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            entity.IsDeleted = true;
            var result = await _admissionRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Admission deleted successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Admission does not deleted." };
        }

        public async Task<ResponseModel> GetAllAdmission(bool? isActive = null)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _admissionRepository.Find(a => a.IsDeleted == false && (!isActive.HasValue || a.IsActive == isActive))
            };
        }

        public async Task<ResponseModel> GetAdmissionPaged(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _admissionRepository.GetAdmissionPaged(_userProviderService.UserClaim, pagination)
            };
        }

        public async Task<ResponseModel> GetAdmissionById(long id)
        {
            var result = await _admissionRepository.SingleOrDefaultAsync(a => a.AdmissionId == id, b => b.Users);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "Admission does not exists." };
            }
        }

        public async Task<ResponseModel> GetAdmission(long schemeId, long intakeId, long instituteId, bool isGts)
        {
            var result = await _admissionRepository.GetAdmission(_userProviderService.UserClaim, schemeId, intakeId, instituteId, isGts);
            if (result != null)
            {
                if (result.ApprovalBy1 == _userProviderService.UserClaim.UserId)
                {
                    result.IsApprovedByUser = result.Approved1;
                }
                if (result.ApprovalBy2 == _userProviderService.UserClaim.UserId)
                {
                    result.IsApprovedByUser = result.Approved2;
                }
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "Admission does not exists." };
            }
        }

        public async Task<ResponseModel> UpdateAdmission(Admission updateEntity, bool isPublish)
        {
            var entityResult = await GetAdmissionById(updateEntity.AdmissionId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as Admission;

            if (isPublish)
            {
                entity.IsPublish = isPublish;
            }
            else
            {
                if (!entity.ApprovalDate1.HasValue && entity.ApprovalBy1 == _userProviderService.UserClaim.UserId)
                {
                    entity.ApprovalDate1 = CommonUtils.GetDefaultDateTime();
                    entity.Approved1 = updateEntity.Approved1;
                    entity.ApprovalRemarks1 = updateEntity.ApprovalRemarks1;
                }
                if (!entity.ApprovalDate2.HasValue && entity.ApprovalBy2 == _userProviderService.UserClaim.UserId)
                {
                    entity.ApprovalDate2 = CommonUtils.GetDefaultDateTime();
                    entity.Approved2 = updateEntity.Approved2;
                    entity.ApprovalRemarks2 = updateEntity.ApprovalRemarks2;
                }
            }

            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;

            CommonUtils.EncodeProperties(entity);
            var result = await _admissionRepository.SaveChangesAsync();

            if (result > 0)
            {
                if (isPublish)
                {
                    var template = await _intakeTemplateService.GetIntakeTemplateByKey(entity.IntakeId, nameof(TemplateKeys.AdmissionEmail));
                    var settings = await _settingService.GetSettingsByKeys(new List<string>{
                        nameof(SettingKeys.ReciepientList),
                        nameof(SettingKeys.OverrideEmail),
                    });
                    var overrideEmail = settings.FirstOrDefault(s => s.SettingKey == nameof(SettingKeys.OverrideEmail)).SettingValue;

                    var scheme = (await _schemeService.GetSchemeById(entity.SchemeId)).Data as Scheme;
                    var intake = (await _intakeService.GetIntakeById(entity.IntakeId)).Data as Intake;
                    var institute = (await _instituteService.GetInstituteById(entity.InstituteId)).Data as Institute;

                    var meritList = new StringBuilder($"<div><h3>Merit List</h3></div><table border='1' cellspacing='5' cellpadding='5' width='100%'><tr><td>Sr. No</td><td>Name</td></tr>");
                    var waitList = new StringBuilder($"<div><h3>Waiting List</h3></div><table border='1' cellspacing='5' cellpadding='5' width='100%'><tr><td>Sr. No</td><td>Name</td></tr>");

                    dynamic filters = new ExpandoObject();
                    filters.schemeId = entity.SchemeId;
                    filters.intakeId = entity.IntakeId;
                    filters.instituteId = entity.InstituteId;
                    var userResult = await GetAdmissionUsers(new Pagination { Filters = filters });
                    var users = userResult.Data.Data as List<AdmissionUserModel>;
                    foreach (var user in users)
                    {
                        if (user.AdmissionStatusId == AdmissionStatuses.Confirm.GetHashCode())
                        {
                            meritList.Append($"<tr><td>{user.Rank}</td><td>{user.StaffName}</td></tr>");
                        }
                        else if (user.AdmissionStatusId == AdmissionStatuses.Waiting.GetHashCode())
                        {
                            waitList.Append($"<tr><td>{user.Rank}</td><td>{user.StaffName}</td></tr>");
                        }
                    }
                    meritList.Append($"</table>");
                    waitList.Append($"</table>");

                    var subject = template.TemplateSubject
                        .Replace(TemplateDataKeys.SchemeName, scheme.SchemeName)
                        .Replace(TemplateDataKeys.IntakeName, intake.IntakeName)
                        .Replace(TemplateDataKeys.InstituteName, institute.InstituteName)
                        ;

                    var body = template.TemplateContent
                        .Replace(TemplateDataKeys.SchemeName, scheme.SchemeName)
                        .Replace(TemplateDataKeys.IntakeName, intake.IntakeName)
                        .Replace(TemplateDataKeys.InstituteName, institute.InstituteName)
                        .Replace(TemplateDataKeys.Meritlist, meritList.ToString())
                        .Replace(TemplateDataKeys.Waitlist, waitList.ToString())
                        ;


                    var emailResult = await _emailSender.SendSmtpEmailAsync(new EmailModel
                    {
                        Subject = subject,
                        To = string.IsNullOrEmpty(overrideEmail) ?
                                string.Join(",", await _userService.GetUserEmails(entity.Users.Select(u => u.UserId).ToList())) :
                                overrideEmail,
                        CC = string.IsNullOrEmpty(overrideEmail) ?
                                settings.FirstOrDefault(s => s.SettingKey == nameof(SettingKeys.ReciepientList)).SettingValue :
                                overrideEmail,
                        Body = body,
                    });
                }

                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Admission updated successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Admission does not updated." };
        }

        public async Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null)
        {
            return (await _admissionRepository.GetDropdwon(id, isActive)).Data;
        }

        public async Task<ResponseModel> GetAdmissionInstitutes(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _admissionRepository.GetAdmissionInstitutes(_userProviderService.UserClaim, pagination)
            };
        }

        public async Task<ResponseModel> GetAdmissionUsers(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _admissionRepository.GetAdmissionUsers(pagination)
            };
        }

        public async Task<ResponseModel> GetConfirmAdmissionUsers(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _admissionRepository.GetConfirmAdmissionUsers(pagination)
            };
        }

        public async Task<ResponseModel> UpdateAdmissionUser(AdmissionUser updateEntity)
        {
            var entityResult = await _admissionUserService.GetAdmissionUserById(updateEntity.AdmissionUserId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as AdmissionUser;
            entity.IsConfirmByEmp = updateEntity.IsConfirmByEmp;
            entity.ConfirmDate = CommonUtils.GetDefaultDateTime();
            entity.EmployeeRemarks = updateEntity.EmployeeRemarks;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;

            var admission = await _admissionRepository.SingleOrDefaultAsync(a => a.AdmissionId == entity.AdmissionId, b => b.Users);
            if (updateEntity.IsConfirmByEmp == true)
            {
                entity.AdmissionStatusId = AdmissionStatuses.Accepted.GetHashCode();
                var nominationResult = await _nominationService.GetNominationById(entity.NominationId);
                var nomination = nominationResult.Data as Nomination;
                nomination.InstitueId = admission.InstituteId;
                nomination.AdmissionStatusId = entity.AdmissionStatusId;
                nomination.AdmissionAcceptedDate = CommonUtils.GetDefaultDateTime();
                nomination.UpdatedDate = CommonUtils.GetDefaultDateTime();
                nomination.UpdatedBy = _userProviderService.UserClaim.UserId;
                foreach (var admissionUser in await _admissionUserService.GetAdmissionUserByNominationId(entity.NominationId))
                {
                    if (admissionUser.AdmissionUserId != entity.AdmissionUserId)
                    {
                        admissionUser.AdmissionStatusId = AdmissionStatuses.Rejected.GetHashCode();
                        var confirmed = AdmissionStatuses.Confirm.GetHashCode();
                        var waiting = AdmissionStatuses.Waiting.GetHashCode();
                        var maxRank = 0;
                        var _admission = await _admissionRepository.SingleOrDefaultAsync(a => a.AdmissionId == admissionUser.AdmissionId, b => b.Users);
                        foreach (var user in _admission.Users.Where(u => u.AdmissionStatusId == confirmed && u.Rank > entity.Rank))
                        {
                            user.Rank--;
                            maxRank = user.Rank;
                        }
                        foreach (var user in _admission.Users.Where(u => u.AdmissionStatusId == waiting))
                        {
                            if (user.Rank == 1)
                            {
                                user.Rank = maxRank + 1;
                                user.AdmissionStatusId = confirmed;
                            }
                            else
                            {
                                user.Rank--;
                            }
                        }
                    }
                }
            }
            else
            {
                entity.AdmissionStatusId = AdmissionStatuses.Rejected.GetHashCode();
                var confirmed = AdmissionStatuses.Confirm.GetHashCode();
                var waiting = AdmissionStatuses.Waiting.GetHashCode();
                var maxRank = 0;
                foreach (var user in admission.Users.Where(u => u.AdmissionStatusId == confirmed && u.Rank > entity.Rank))
                {
                    user.Rank--;
                    maxRank = user.Rank;
                }
                foreach (var user in admission.Users.Where(u => u.AdmissionStatusId == waiting))
                {
                    if (user.Rank == 1)
                    {
                        user.Rank = maxRank + 1;
                        user.AdmissionStatusId = confirmed;
                    }
                    else
                    {
                        user.Rank--;
                    }
                }
            }

            CommonUtils.EncodeProperties(entity);
            var result = await _admissionRepository.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Admission updated successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Admission does not updated." };
        }

        public async Task<ResponseModel> ConfirmAdmissions(ICollection<AdmissionUserModel> admissionUsers)
        {
            if (!admissionUsers.Any())
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status204NoContent, Message = "No Admission uploaded" };
            }

            var entity = await _admissionRepository.SingleOrDefaultAsync(a => a.AdmissionId == admissionUsers.FirstOrDefault().AdmissionId, b => b.Users);
            var intake = (await _intakeService.GetIntakeById(entity.IntakeId)).Data as Intake;
            var institute = intake.Institutes.FirstOrDefault(i => i.InstituteId == entity.InstituteId);
            if (institute.TotalSeats < admissionUsers.Count)
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status204NoContent, Message = "Admissions are more than Total seat" };
            }

            var result = 0;
            foreach (var batch in admissionUsers.Batch(100))
            {
                foreach (var admissionUser in batch)
                {
                    var user = entity.Users.FirstOrDefault(a => a.AdmissionUserId == admissionUser.AdmissionUserId);
                    user.AdmissionStatusId = AdmissionStatuses.Active.GetHashCode();
                    user.IsConfirmByInstitute = true;
                    user.Semester = admissionUser.Semester;
                    user.UpdatedDate = CommonUtils.GetDefaultDateTime();
                    user.UpdatedBy = _userProviderService.UserClaim.UserId;
                    CommonUtils.EncodeProperties(user);
                }
                result = await _admissionRepository.SaveChangesAsync();
            }

            entity.IsConfirmUpload = true;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;

            CommonUtils.EncodeProperties(entity);
            result = await _admissionRepository.SaveChangesAsync();
            if (result > 0)
            {
                var batch = new Batch
                {
                    AdmissionId = entity.AdmissionId,
                    InstituteId = entity.InstituteId,
                    IntakeId = entity.IntakeId,
                    SchemeId = entity.SchemeId,
                    StartDate = intake.StartDate,
                    TotalSeats = institute.TotalSeats,
                    TotalFee = 0,
                    Users = admissionUsers.Select(u => new BatchUser { UserId = u.UserId }).ToList()
                };
                var batchResult = await _batchService.CreateBatch(batch);

                var template = await _intakeTemplateService.GetIntakeTemplateByKey(entity.IntakeId, nameof(TemplateKeys.AdmissionConfirmEmail));
                var settings = await _settingService.GetSettingsByKeys(new List<string>{
                    nameof(SettingKeys.OverrideEmail),
                });

                var overrideEmail = settings.FirstOrDefault(s => s.SettingKey == nameof(SettingKeys.OverrideEmail)).SettingValue;
                var managers = await _userService.GetUserLeadEmails(admissionUsers.Select(u => u.UserId).ToList());

                var scheme = (await _schemeService.GetSchemeById(entity.SchemeId)).Data as Scheme;
                var _institute = (await _instituteService.GetInstituteById(entity.InstituteId)).Data as Institute;

                var confirmationList = new StringBuilder($"<div><h3>Confirmation List</h3></div><table border='1' cellspacing='5' cellpadding='5' width='100%'><tr><td>Sr. No</td><td>Name</td></tr>");

                dynamic filters = new ExpandoObject();
                filters.schemeId = entity.SchemeId;
                filters.intakeId = entity.IntakeId;
                filters.instituteId = entity.InstituteId;
                var userResult = await GetConfirmAdmissionUsers(new Pagination { Filters = filters });
                var users = userResult.Data.Data as List<ConfirmAdmissionUserModel>;
                foreach (var user in users)
                {
                    confirmationList.Append($"<tr><td>{user.Rank}</td><td>{user.StaffName}</td></tr>");
                }
                confirmationList.Append($"</table>");

                var subject = template.TemplateSubject
                    .Replace(TemplateDataKeys.SchemeName, scheme.SchemeName)
                    .Replace(TemplateDataKeys.IntakeName, intake.IntakeName)
                    .Replace(TemplateDataKeys.InstituteName, _institute.InstituteName)
                    ;

                var body = template.TemplateContent
                    .Replace(TemplateDataKeys.SchemeName, scheme.SchemeName)
                    .Replace(TemplateDataKeys.IntakeName, intake.IntakeName)
                    .Replace(TemplateDataKeys.InstituteName, _institute.InstituteName)
                    .Replace(TemplateDataKeys.ConfirmationList, confirmationList.ToString())
                    ;

                var emailResult = await _emailSender.SendSmtpEmailAsync(new EmailModel
                {
                    Subject = subject,
                    To = string.IsNullOrEmpty(overrideEmail) ?
                            string.Join(",", await _userService.GetUserEmails(admissionUsers.Select(u => u.UserId).ToList())) :
                            overrideEmail,
                    CC = string.IsNullOrEmpty(overrideEmail) ?
                            string.Join(",", managers.Select(u => u.Email).ToList()) :
                            overrideEmail,
                    Body = body
                });

                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Admissions confirmed successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Admission does not confirmed." };
        }

        public async Task<ResponseModel> UpdateAdmissionUserStatus(AdmissionUser updateEntity)
        {
            var entityResult = await _admissionUserService.GetAdmissionUserById(updateEntity.AdmissionUserId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as AdmissionUser;
            entity.AdmissionStatusId = updateEntity.AdmissionStatusId;
            entity.ApproverRemarks = updateEntity.ApproverRemarks;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;

            var nominationResult = await _nominationService.GetNominationById(entity.NominationId);
            var nomination = nominationResult.Data as Nomination;
            nomination.AdmissionStatusId = entity.AdmissionStatusId;
            nomination.UpdatedDate = CommonUtils.GetDefaultDateTime();
            nomination.UpdatedBy = _userProviderService.UserClaim.UserId;

            CommonUtils.EncodeProperties(entity);
            var result = await _admissionRepository.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Admission updated successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Admission does not updated." };
        }

        public async Task<ResponseModel> UpdateAdmissionUserLegal(AdmissionUser updateEntity)
        {
            var entityResult = await _admissionUserService.GetAdmissionUserById(updateEntity.AdmissionUserId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as AdmissionUser;
            entity.IsBondAccepted = true;
            entity.BondAcceptedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            CommonUtils.EncodeProperties(entity);
            var result = await _admissionRepository.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Admission updated successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Admission does not updated." };
        }

        public async Task<int> GetTotalAdmission(dynamic filters)
        {
            return await _admissionRepository.GetTotalAdmission(filters);
        }

        public async Task<int> GetTotalWaillist(dynamic filters)
        {
            return await _admissionRepository.GetTotalWaillist(filters);
        }

        public async Task<int> GetPendingServiceAgreement(dynamic filters)
        {
            return await _admissionRepository.GetPendingServiceAgreement(filters);
        }

        public async Task<int> GetPendingAdmissionConfirmation(dynamic filters)
        {
            return await _admissionRepository.GetPendingAdmissionConfirmation(filters);
        }

        public async Task<ResponseModel> GetAdmissionPagedByDesg(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _admissionRepository.GetAdmissionPagedByDesg(pagination)
            };
        }

        public async Task<ResponseModel> GetAdmissionPagedByDiv(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _admissionRepository.GetAdmissionPagedByDiv(pagination)
            };
        }


    }
}
