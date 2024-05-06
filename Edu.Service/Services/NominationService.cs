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
using Microsoft.IdentityModel.Tokens;
using Edu.Repository.Repositories;

namespace Edu.Service.Services
{
    public class NominationService : INominationService
    {
        private readonly INominationRepository _nominationRepository;
        private readonly IUserProviderService _userProviderService;
        private readonly ISettingService _settingService;
        private readonly IIntakeTemplateService _intakeTemplateService;
        private readonly IEmailSender _emailSender;
        private readonly IUserService _userService;
        private readonly ISchemeService _schemeService;
        private readonly IIntakeService _intakeService;
        private readonly IFileService _fileService;
        private readonly IInstituteService _instituteService;

        public NominationService(
            INominationRepository nominationRepository,
            IUserProviderService userProviderService,
            ISettingService settingService,
            IIntakeTemplateService intakeTemplateService,
            IEmailSender emailSender,
            IUserService userService,
            ISchemeService schemeService,
            IIntakeService intakeService,
            IFileService fileService,
            IInstituteService instituteService
        )
        {
            _nominationRepository = nominationRepository;
            _userProviderService = userProviderService;
            _settingService = settingService;
            _intakeTemplateService = intakeTemplateService;
            _emailSender = emailSender;
            _userService = userService;
            _schemeService = schemeService;
            _intakeService = intakeService;
            _fileService = fileService;
            _instituteService = instituteService;
        }

        public async Task<ResponseModel> CreateNomination(Nomination entity)
        {
            entity.CreatedBy = _userProviderService.UserClaim.UserId;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            CommonUtils.EncodeProperties(entity);
            await _nominationRepository.AddAsync(entity);
            var result = await _nominationRepository.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Nomination created successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Nomination does not created." };
        }

        public async Task<ResponseModel> CreateNominations(ICollection<Nomination> entities)
        {

            var first = entities.FirstOrDefault();
            var existNominations = await _nominationRepository.Find(nr => nr.IntakeId == first.IntakeId && nr.IntakeId == first.IntakeId);
            foreach (var batch in existNominations.Batch(100))
            {
                _nominationRepository.RemoveRange(batch);
                await _nominationRepository.SaveChangesAsync();
            }

            var result = 0;
            foreach (var batch in entities.Batch(100))
            {
                foreach (var entity in batch)
                {
                    entity.CreatedBy = _userProviderService.UserClaim.UserId;
                    entity.UpdatedBy = _userProviderService.UserClaim.UserId;
                    CommonUtils.EncodeProperties(entity);
                    await _nominationRepository.AddAsync(entity);
                }
                result = await _nominationRepository.SaveChangesAsync();
            }

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Nominations created successfully.",
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Nominations does not created." };
        }

        public async Task<ResponseModel> DeleteNomination(long id)
        {
            var entityResult = await GetNominationById(id);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as Nomination;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            entity.IsDeleted = true;
            var result = await _nominationRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Nomination deleted successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Nomination does not deleted." };
        }

        public async Task<ResponseModel> GetAllNomination(bool? isActive = null)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _nominationRepository.Find(a => a.IsDeleted == false && (!isActive.HasValue || a.IsActive == isActive))
            };
        }

        public async Task<ResponseModel> GetEligibilitiesPaged(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _nominationRepository.GetEligibilitiesPaged(_userProviderService.UserClaim, pagination)
            };
        }

        public async Task<ResponseModel> GetNominationPaged(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _nominationRepository.GetNominationPaged(_userProviderService.UserClaim, pagination)
            };
        }

        public async Task<ResponseModel> GetNominationApproverPaged(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _nominationRepository.GetNominationApproverPaged(_userProviderService.UserClaim, pagination)
            };
        }

        public async Task<ResponseModel> GetNominationById(long id)
        {
            var result = await _nominationRepository.SingleOrDefaultAsync(a => a.NominationId == id, b => b.Institutes);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "Nomination does not exists." };
            }
        }

        public async Task<ResponseModel> UpdateNomination(Nomination updateEntity)
        {
            var entityResult = await GetNominationById(updateEntity.NominationId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as Nomination;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;

            CommonUtils.EncodeProperties(entity);
            var result = await _nominationRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Nomination updated successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Nomination does not updated." };
        }

        public async Task<ResponseModel> UpdateNomination(NominationFormModel updateEntity)
        {
            var entityResult = await GetNominationById(updateEntity.NominationId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as Nomination;
            entity.MobileNo = updateEntity.MobileNo;
            entity.Gender = updateEntity.Gender;
            entity.NominationStatusId = updateEntity.NominationStatusId;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;

            var institutes = new List<NominationInstitute>();
            if (!string.IsNullOrEmpty(updateEntity.SelectedInstitutes))
            {
                foreach (var instituteId in updateEntity.SelectedInstitutes.Split(',').Select(long.Parse).ToList())
                {
                    var institute = entity.Institutes.FirstOrDefault(ur => ur.InstituteId == instituteId);
                    if (institute != null)
                    {
                        institutes.Add(institute);
                    }
                    else
                    {
                        institutes.Add(new NominationInstitute { InstituteId = instituteId });
                    }
                }
            }

            entity.Institutes = institutes;

            CommonUtils.EncodeProperties(entity);
            var result = await _nominationRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Nomination updated successfully.",
                    Data = (await GetNominationModel(updateEntity.IntakeId)).Data
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Nomination does not updated." };
        }

        public async Task<ResponseModel> UpdateNomination(List<long> ids, int type, string remarks)
        {
            var apr1Status = 0;
            var apr2Status = 0;
            switch (type)
            {
                case 1:
                    apr1Status = NominationStatuses.DepApprove.GetHashCode();
                    apr2Status = NominationStatuses.DivApprove.GetHashCode();
                    break;
                case 2:
                    apr1Status = NominationStatuses.DepReview.GetHashCode();
                    apr2Status = NominationStatuses.DivReview.GetHashCode();
                    break;
                case 3:
                    apr1Status = NominationStatuses.DepRejected.GetHashCode();
                    apr2Status = NominationStatuses.DivRejected.GetHashCode();
                    break;
            }
            var entityResult = await _nominationRepository.Find(n => ids.Contains(n.NominationId));
            foreach (var entity in entityResult)
            {
                if (entity.ApprovalBy1.HasValue && entity.ApprovalBy1.Value == _userProviderService.UserClaim.UserId)
                {
                    entity.ApprovalDate1 = type == 1 ? CommonUtils.GetDefaultDateTime() : null;
                    entity.ApprovalRemarks1 = remarks;
                    entity.NominationStatusId = apr1Status;
                }
                if (entity.ApprovalBy2.HasValue && entity.ApprovalBy2.Value == _userProviderService.UserClaim.UserId)
                {
                    entity.ApprovalDate2 = type == 1 ? CommonUtils.GetDefaultDateTime() : null;
                    entity.ApprovalRemarks2 = remarks;
                    entity.NominationStatusId = apr2Status;
                }
                entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
                entity.UpdatedBy = _userProviderService.UserClaim.UserId;
                CommonUtils.EncodeProperties(entity);
            }

            var result = await _nominationRepository.SaveChangesAsync();

            if (result > 0)
            {
                await NominationApprover2ReminderEmails(entityResult.Where(s => s.NominationStatusId == apr1Status).Select(n => n.NominationId).ToList());

                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Nomination updated successfully.",
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Nomination does not updated." };
        }

        public async Task<ResponseModel> UpdateNominationScore(Nomination updateEntity)
        {
            var entityResult = await GetNominationById(updateEntity.NominationId);
            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as Nomination;
            entity.Score = updateEntity.Score;
            entity.IsExamTaken = updateEntity.IsExamTaken;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            CommonUtils.EncodeProperties(entity);

            var result = await _nominationRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Nomination updated successfully.",
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Nomination does not updated." };
        }

        public async Task<ResponseModel> UpdateNominationScores(List<Nomination> updateEntities)
        {
            var nominationIds = updateEntities.Select(ni => ni.NominationId).ToList();
            var entities = await _nominationRepository.Find(n => nominationIds.Contains(n.NominationId));

            var result = 0;
            foreach (var batch in entities.Batch(100))
            {
                foreach (var entity in batch)
                {
                    var _nomination = updateEntities.FirstOrDefault(n => n.NominationId == entity.NominationId);
                    entity.Score = _nomination.Score;
                    entity.IsExamTaken = true;
                    entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
                    entity.UpdatedBy = _userProviderService.UserClaim.UserId;
                }
                result = await _nominationRepository.SaveChangesAsync();
            }

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Scores uploaded successfully.",
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Scores does not uploaded." };
        }

        public async Task<ResponseModel> UpdateNominationStatus(Nomination updateEntity)
        {
            var entityResult = await GetNominationById(updateEntity.NominationId);
            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as Nomination;
            entity.NominationStatusId = updateEntity.NominationStatusId;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            CommonUtils.EncodeProperties(entity);

            var result = await _nominationRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Nomination updated successfully.",
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Nomination does not updated." };
        }

        public async Task<ResponseModel> UpdateNominationScoreApproval(Nomination updateEntity)
        {
            var entityResult = await GetNominationById(updateEntity.NominationId);
            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as Nomination;
            entity.IsScoreApprove = updateEntity.IsScoreApprove;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            CommonUtils.EncodeProperties(entity);

            var result = await _nominationRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Nomination updated successfully.",
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Nomination does not updated." };
        }

        public async Task<List<DropdownModel>> GetDropdwon(long? id = null, bool? isActive = null)
        {
            return (await _nominationRepository.GetDropdwon(id, isActive)).Data;
        }

        public async Task<ResponseModel> PublishEligibilities(long schemeId, long intakeId)
        {
            var entities = await _nominationRepository.Find(n => n.IsPublish == false && n.SchemeId == schemeId && n.IntakeId == intakeId);
            foreach (var entity in entities)
            {
                entity.NominationStatusId = NominationStatuses.Submitted.GetHashCode();
                entity.IsPublish = true;
                entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
                entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            }

            var result = await _nominationRepository.SaveChangesAsync();
            if (result > 0)
            {
                var template = await _intakeTemplateService.GetIntakeTemplateByKey(intakeId, nameof(TemplateKeys.EligibilityEmail));
                var settings = await _settingService.GetSettingsByKeys(new List<string>{
                    nameof(SettingKeys.ReciepientList),
                    nameof(SettingKeys.OverrideEmail),
                });
                var overrideEmail = settings.FirstOrDefault(s => s.SettingKey == nameof(SettingKeys.OverrideEmail)).SettingValue;

                var scheme = (await _schemeService.GetSchemeById(schemeId)).Data as Scheme;
                var intake = (await _intakeService.GetIntakeById(intakeId)).Data as Intake;
                var attachments = new List<EmailAttachment>();
                if (!string.IsNullOrEmpty(intake.BrochureFilePath))
                {
                    attachments.Add(new EmailAttachment
                    {
                        FileName = intake.BrochureFileName,
                        FileBytes = await _fileService.ReadAllBytesAsync(_fileService.GetAbsolutePath(intake.BrochureFilePath))
                    });
                }
                var intakeInstituteIds = intake.Institutes.Select(i => i.InstituteId);
                var institutes = await _instituteService.GetDropdwon();

                var instituteList = string.Join(", ", institutes.Where(i => intakeInstituteIds.Contains(i.Id)).Select(i => i.Text));
                var instituteNameSeatList = new StringBuilder("<ul>");

                foreach (var institute in intake.Institutes)
                {
                    instituteNameSeatList.Append($"<li>{institutes.FirstOrDefault(i => i.Id == institute.InstituteId).Text} - {institute.TotalSeats} Seats</li>");
                }
                instituteNameSeatList.Append($"</ul>");

                var subject = template.TemplateSubject
                    .Replace(TemplateDataKeys.SchemeName, scheme.SchemeName)
                    .Replace(TemplateDataKeys.IntakeName, intake.IntakeName);

                var body = template.TemplateContent
                    .Replace(TemplateDataKeys.SchemeName, scheme.SchemeName)
                    .Replace(TemplateDataKeys.IntakeName, intake.IntakeName)
                    .Replace(TemplateDataKeys.InstituteList, instituteList.ToString())
                    .Replace(TemplateDataKeys.InstituteNameSeatList, instituteNameSeatList.ToString())
                    .Replace(TemplateDataKeys.NominationCutoffDate, intake.NominationCutoffDate.FormatDateOrdinal())
                    .Replace(TemplateDataKeys.ExamDate, intake.ExamDate.FormatDateOrdinal())
                    ;


                var emailResult = await _emailSender.SendSmtpEmailAsync(new EmailModel
                {
                    Subject = subject,
                    CC = string.IsNullOrEmpty(overrideEmail) ?
                            settings.FirstOrDefault(s => s.SettingKey == nameof(SettingKeys.ReciepientList)).SettingValue :
                            overrideEmail,
                    Bcc = string.IsNullOrEmpty(overrideEmail) ?
                            string.Join(",", await _userService.GetUserEmails(entities.Select(u => u.UserId).ToList())) :
                            overrideEmail,
                    Body = body,
                    Attachments = attachments,
                });

                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Nominations published successfully.",
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Nominations does not published." };
        }

        public async Task<ResponseModel> GetNominationModel(long intakeId)
        {
            var result = await _nominationRepository.GetNominationModel(_userProviderService.UserClaim, intakeId);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "Nomination does not exists." };
            }
        }

        public async Task<ResponseModel> GetScorecardPaged(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _nominationRepository.GetScorecardPaged(pagination)
            };
        }

        public async Task<ResponseModel> GetScorecardModel(long intakeId)
        {
            var result = await _nominationRepository.GetScorecardModel(_userProviderService.UserClaim, intakeId);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "Nomination does not exists." };
            }
        }

        public async Task<bool> AllowGtsScorecard(long intakeId)
        {
            return await _nominationRepository.AllowGtsScorecard(_userProviderService.UserClaim, intakeId);
        }

        public async Task<int> GetTotalEligible(dynamic filters)
        {
            return await _nominationRepository.GetTotalEligible(filters);
        }

        public async Task<int> GetTotalNomination(dynamic filters)
        {
            return await _nominationRepository.GetTotalNomination(filters);
        }

        public async Task<int> GetTotalInstitute(dynamic filters)
        {
            return await _nominationRepository.GetTotalInstitute(filters);
        }

        public async Task<int> GetPendingNominationAcceptance(dynamic filters)
        {
            return await _nominationRepository.GetPendingNominationAcceptance(filters);
        }

        public async Task<ResponseModel> GetEmployeeNomination(long schemeId, long intakeId, long userId)
        {
            var result = await _nominationRepository.GetEmployeeNomination(schemeId, intakeId, userId);
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "Nomination does not exists." };
            }
        }

        public async Task<List<long>> GetNominationUsers(long schemeId, long intakeId)
        {
            return await _nominationRepository.GetNominationUsers(schemeId, intakeId);
        }

        public async Task PendingScorecardReminderEmails()
        {
            var intakes = await _intakeService.GetAllScorecardIntake();
            if (!intakes.Any()) { return; }
            var users = await _nominationRepository.GetAllPendingScorecardUser(intakes.Select(i => i.IntakeId).ToList());
            var managers = await _userService.GetUserLeadEmails(users.Select(i => i.Id).ToList());
            var templates = await _intakeTemplateService.GetIntakeTemplatesByKey(intakes.Select(i => i.IntakeId).ToList(), nameof(TemplateKeys.PendingScorecardEmail));
            var settings = await _settingService.GetSettingsByKeys(new List<string>{
                    nameof(SettingKeys.OverrideEmail),
                });

            var overrideEmail = settings.FirstOrDefault(s => s.SettingKey == nameof(SettingKeys.OverrideEmail)).SettingValue;
            var schemes = await _schemeService.GetDropdwon();

            foreach (var emailUser in users)
            {
                var template = templates.FirstOrDefault(i => i.IntakeId == emailUser.IntakeId);
                var intake = intakes.FirstOrDefault(i => i.IntakeId == emailUser.IntakeId);
                var scheme = schemes.FirstOrDefault(i => i.Id == emailUser.SchemeId);

                var subject = template.TemplateSubject
                       .Replace(TemplateDataKeys.SchemeName, scheme.Text)
                       .Replace(TemplateDataKeys.IntakeName, intake.IntakeName)
                       ;

                var body = template.TemplateContent
                    .Replace(TemplateDataKeys.SchemeName, scheme.Text)
                    .Replace(TemplateDataKeys.IntakeName, intake.IntakeName)
                    .Replace(TemplateDataKeys.ScoreCutoffDate, intake.ScorecardCutoffDate.FormatDateOrdinal())
                    ;

                var emailResult = await _emailSender.SendSmtpEmailAsync(new EmailModel
                {
                    Subject = subject,
                    To = string.IsNullOrEmpty(overrideEmail) ? emailUser.Email : overrideEmail,
                    CC = string.IsNullOrEmpty(overrideEmail) ?
                            string.Join(",", managers.Where(m => m.Id == emailUser.Id).Select(u => u.Email).ToList()) :
                            overrideEmail,
                    Body = body
                });
            }
        }

        public async Task NominationApprover1ReminderEmails()
        {
            var nominations = await _nominationRepository.GetNominationApprover1();
            if (!nominations.Any()) { return; }

            var template = await _intakeTemplateService.GetIntakeTemplateByKey(nominations.FirstOrDefault().IntakeId, nameof(TemplateKeys.NominationApprover1));
            var settings = await _settingService.GetSettingsByKeys(new List<string>{
                    nameof(SettingKeys.OverrideEmail),
                });

            var overrideEmail = settings.FirstOrDefault(s => s.SettingKey == nameof(SettingKeys.OverrideEmail)).SettingValue;
            var schemes = await _schemeService.GetDropdwon();

            foreach (var approverEmail in nominations.GroupBy(n => n.ApproverEmail))
            {
                var nominationTable = new StringBuilder();
                nominationTable.Append($"<table border='1' cellspacing='5' cellpadding='5' width='100%'><tr><td>Staff Id</td><td>Staff Name</td><td>Scheme Name</td></tr>");
                foreach (var nomination in approverEmail.ToList())
                {
                    nominationTable.Append($"<tr><td>{nomination.MsilUserId}</td><td>{nomination.StaffName}</td><td>{nomination.SchemeName} - {nomination.IntakeName}</td></tr>");
                }

                var subject = template.TemplateSubject;

                var body = template.TemplateContent
                    .Replace(TemplateDataKeys.NominationApprover1Name, approverEmail.FirstOrDefault().Approver1Name)
                    .Replace(TemplateDataKeys.NominationStaffList, nominationTable.ToString())
                    ;

                var emailResult = await _emailSender.SendSmtpEmailAsync(new EmailModel
                {
                    Subject = subject,
                    To = string.IsNullOrEmpty(overrideEmail) ? approverEmail.Key : overrideEmail,
                    Body = body
                });
            }
        }

        private async Task NominationApprover2ReminderEmails(List<long> ids)
        {
            if (!ids.Any()) { return; }

            var nominations = await _nominationRepository.GetNominationApprover2(ids);
            if (!nominations.Any()) { return; }

            var template = await _intakeTemplateService.GetIntakeTemplateByKey(nominations.FirstOrDefault().IntakeId, nameof(TemplateKeys.NominationApprover2));
            var settings = await _settingService.GetSettingsByKeys(new List<string>{
                    nameof(SettingKeys.OverrideEmail),
                });

            var overrideEmail = settings.FirstOrDefault(s => s.SettingKey == nameof(SettingKeys.OverrideEmail)).SettingValue;
            var schemes = await _schemeService.GetDropdwon();

            foreach (var approverEmail in nominations.GroupBy(n => n.ApproverEmail))
            {
                var nominationTable = new StringBuilder();
                nominationTable.Append($"<table border='1' cellspacing='5' cellpadding='5' width='100%'><tr><td>Staff Id</td><td>Staff Name</td><td>Scheme Name</td></tr>");
                foreach (var nomination in approverEmail.ToList())
                {
                    nominationTable.Append($"<tr><td>{nomination.MsilUserId}</td><td>{nomination.StaffName}</td><td>{nomination.SchemeName} - {nomination.IntakeName}</td></tr>");
                }

                var subject = template.TemplateSubject;

                var body = template.TemplateContent
                    .Replace(TemplateDataKeys.NominationApprover1Name, approverEmail.FirstOrDefault().Approver1Name)
                    .Replace(TemplateDataKeys.NominationApprover2Name, approverEmail.FirstOrDefault().Approver2Name)
                    .Replace(TemplateDataKeys.NominationStaffList, nominationTable.ToString())
                    ;

                var emailResult = await _emailSender.SendSmtpEmailAsync(new EmailModel
                {
                    Subject = subject,
                    To = string.IsNullOrEmpty(overrideEmail) ? approverEmail.Key : overrideEmail,
                    Body = body
                });
            }
        }
    }
}
