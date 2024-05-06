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
using System.Xml.Linq;

namespace Edu.Service.Services
{
    public class IntakeService : IIntakeService
    {
        private readonly IIntakeRepository _intakeRepository;
        private readonly ITemplateService _templateService;
        private readonly IUserProviderService _userProviderService;

        public IntakeService(
            IIntakeRepository intakeRepository,
            ITemplateService templateService,
            IUserProviderService userProviderService
        )
        {
            _intakeRepository = intakeRepository;
            _templateService = templateService;
            _userProviderService = userProviderService;
        }

        public async Task<ResponseModel> CreateIntake(Intake entity)
        {
            foreach (var locationId in entity.SelectedLocations.Split(',').Select(long.Parse).ToList())
            {
                entity.Locations.Add(new IntakeLocation { LocationId = locationId });
            }
            foreach (var documentTypeId in entity.SelectedDocumentTypes.Split(',').Select(long.Parse).ToList())
            {
                entity.DocumentTypes.Add(new IntakeDocumentType { DocumentTypeId = documentTypeId });
            }
            foreach (Template template in (await _templateService.GetAllTemplate()).Data)
            {
                entity.Templates.Add(new IntakeTemplate
                {
                    TemplateId = template.TemplateId,
                    TemplateSubject = template.TemplateSubject,
                    TemplateContent = template.TemplateContent,
                });
            }

            entity.CreatedBy = _userProviderService.UserClaim.UserId;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            CommonUtils.EncodeProperties(entity);
            await _intakeRepository.AddAsync(entity);
            var result = await _intakeRepository.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Intake created successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Intake does not created." };
        }

        public async Task<ResponseModel> DeleteIntake(long id)
        {
            var entityResult = await GetIntakeById(id);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as Intake;
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;
            entity.IsDeleted = true;
            var result = await _intakeRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Intake deleted successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Intake does not deleted." };
        }

        public async Task<ResponseModel> GetAllIntake(bool? isActive = null)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _intakeRepository.Find(a => a.IsDeleted == false && (!isActive.HasValue || a.IsActive == isActive))
            };
        }

        public async Task<ResponseModel> GetIntakePaged(Pagination pagination)
        {
            return new ResponseModel
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = await _intakeRepository.GetIntakePaged(pagination)
            };
        }

        public async Task<ResponseModel> GetIntakeById(long id)
        {
            var result = await _intakeRepository.SingleOrDefaultAsync(
                a => a.IntakeId == id,
                b => b.Locations,
                c => c.Institutes,
                d => d.DocumentTypes
            );
            if (result != null)
            {
                return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Data = result };
            }
            else
            {
                return new ResponseModel { Success = false, StatusCode = StatusCodes.Status404NotFound, Message = "Intake does not exists." };
            }
        }

        public async Task<ResponseModel> UpdateIntake(Intake updateEntity)
        {
            var entityResult = await GetIntakeById(updateEntity.IntakeId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as Intake;
            entity.IntakeName = updateEntity.IntakeName;
            entity.SchemeId = updateEntity.SchemeId;
            entity.StartDate = updateEntity.StartDate;
            entity.ScorecardCutoffDate = updateEntity.ScorecardCutoffDate;
            entity.ExamDate = updateEntity.ExamDate;
            entity.NominationCutoffDate = updateEntity.NominationCutoffDate;
            entity.ScorecardCutoffDate = updateEntity.ScorecardCutoffDate;
            entity.IsGTSScoreUpload = updateEntity.IsGTSScoreUpload;
            entity.UpdatedDate = CommonUtils.GetDefaultDateTime();
            entity.UpdatedBy = _userProviderService.UserClaim.UserId;

            SetIntakeMultiSelect(entity, updateEntity);

            CommonUtils.EncodeProperties(entity);
            var result = await _intakeRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Intake updated successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Intake does not updated." };
        }

        public async Task<ResponseModel> UpdateIntakeBrochure(Intake updateEntity)
        {
            var entityResult = await GetIntakeById(updateEntity.IntakeId);

            if (!entityResult.Success) { return entityResult; }

            var entity = entityResult.Data as Intake;
            entity.BrochureFileName = updateEntity.BrochureFileName;
            entity.BrochureFilePath = updateEntity.BrochureFilePath;
            CommonUtils.EncodeProperties(entity);
            var result = await _intakeRepository.SaveChangesAsync();

            if (result > 0)
            {
                return new ResponseModel
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Intake updated successfully.",
                    Data = entity
                };
            }
            return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Intake does not updated." };
        }

        public async Task<List<DropdownModel>> GetDropdwon(long? id = null, long? schemeId = null, bool? isActive = null)
        {
            return (await _intakeRepository.GetDropdwon(id, schemeId, isActive)).Data;
        }

        public async Task<List<DropdownModel>> GetEmployeeDropdwon(long schemeId, long userId)
        {
            return (await _intakeRepository.GetEmployeeDropdwon(schemeId, userId)).Data;
        }

        public async Task<IEnumerable<Intake>> GetAllScorecardIntake()
        {
            var today = DateTime.Now.Date;
            return await _intakeRepository.Find(a => a.IsDeleted == false
                                                && a.ExamDate.Date <= today
                                                && a.ScorecardCutoffDate.Date >= today);
        }

        private void SetIntakeMultiSelect(Intake entity, Intake updateEntity)
        {
            var locations = new List<IntakeLocation>();
            foreach (var locationId in updateEntity.SelectedLocations.Split(',').Select(long.Parse).ToList())
            {
                var location = entity.Locations.FirstOrDefault(ur => ur.LocationId == locationId);
                if (location != null)
                {
                    locations.Add(location);
                }
                else
                {
                    locations.Add(new IntakeLocation { LocationId = locationId });
                }
            }

            var institutes = new List<IntakeInstitute>();
            foreach (var instituteId in updateEntity.SelectedInstitutes.Split(',').Select(long.Parse).ToList())
            {
                var updateInsititute = updateEntity.Institutes.FirstOrDefault(ur => ur.InstituteId == instituteId);
                var institute = entity.Institutes.FirstOrDefault(ur => ur.InstituteId == instituteId);
                if (institute != null)
                {
                    institute.TotalSeats = updateInsititute.TotalSeats;
                    institute.AdmissionCutoffDate = updateInsititute.AdmissionCutoffDate;
                    institute.UpdatedDate = CommonUtils.GetDefaultDateTime();
                    institute.UpdatedBy = _userProviderService.UserClaim.UserId;
                    institutes.Add(institute);
                }
                else
                {
                    institutes.Add(updateInsititute);
                }
            }

            var documentTypes = new List<IntakeDocumentType>();
            foreach (var documentTypeId in updateEntity.SelectedDocumentTypes.Split(',').Select(long.Parse).ToList())
            {
                var documentType = entity.DocumentTypes.FirstOrDefault(ur => ur.DocumentTypeId == documentTypeId);
                if (documentType != null)
                {
                    documentTypes.Add(documentType);
                }
                else
                {
                    documentTypes.Add(new IntakeDocumentType { DocumentTypeId = documentTypeId });
                }
            }

            entity.Locations = locations;
            entity.Institutes = institutes;
            entity.DocumentTypes = documentTypes;
        }


    }
}
