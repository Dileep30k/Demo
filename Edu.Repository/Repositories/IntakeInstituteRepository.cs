using Core.Repository;
using Core.Repository.Models;
using Core.Utility.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edu.Abstraction.ComplexModels;
using Edu.Abstraction.Models;
using Edu.Repository.Contexts;
using Edu.Repository.Interfaces;

namespace Edu.Repository.Repositories
{
    public class IntakeInstituteRepository : Repository<IntakeInstitute>, IIntakeInstituteRepository
    {
        public IntakeInstituteRepository(EduDbContext context)
            : base(context)
        { }

        private EduDbContext EduDbContext
        {
            get { return _dbContext as EduDbContext; }
        }

        public async Task<PagedList> GetIntakeInstitutePaged(UserClaim userClaim, Pagination pagination)
        {
            long? intakeId = IsPropertyExist(pagination.Filters, "intakeId") ? pagination.Filters?.intakeId : null;

            IRepository<IntakeInstituteModel> repository = new Repository<IntakeInstituteModel>(EduDbContext);
            var query = (from ii in EduDbContext.IntakeInstitutes
                         join intake in EduDbContext.Intakes on ii.IntakeId equals intake.IntakeId
                         join scheme in EduDbContext.Schemes on intake.SchemeId equals scheme.SchemeId
                         join ins in EduDbContext.Institutes on ii.InstituteId equals ins.InstituteId
                         join nom in EduDbContext.Nominations on new { intake.IntakeId, intake.SchemeId } equals new { nom.IntakeId, nom.SchemeId }
                         where !ii.IsDeleted
                         && (!intakeId.HasValue || intakeId.Value == ii.IntakeId)
                         && (nom.IsPublish == true)
                         && (nom.UserId == userClaim.UserId)
                         select new IntakeInstituteModel()
                         {
                             IntakeInstituteId = ii.IntakeInstituteId,
                             SchemeName = scheme.SchemeName,
                             InstituteName = ins.InstituteName,
                             TotalSeats = ii.TotalSeats,
                             AdmissionCutoffDate = ii.AdmissionCutoffDate,
                             NominationCutoffDate = intake.NominationCutoffDate,
                             StartDate = intake.StartDate,
                             ExamDate = intake.ExamDate,
                         });

            return await repository.GetPagedReponseAsync(pagination, null, query);
        }

        public async Task<PagedList> GetDropdwon(long? id = null, long? intakeId = null)
        {
            Repository<DropdownModel> repository = new(EduDbContext);
            var query = (from r in EduDbContext.IntakeInstitutes
                         join ins in EduDbContext.Institutes on r.InstituteId equals ins.InstituteId
                         where !r.IsDeleted &&
                         (!id.HasValue || r.IntakeInstituteId == id) &&
                         (!intakeId.HasValue || r.IntakeId == intakeId)
                         select new DropdownModel()
                         {
                             Id = ins.InstituteId,
                             Text = ins.InstituteName,
                             IsActive = ins.IsActive
                         });

            return await repository.GetPagedReponseAsync(new Pagination { SortOrderColumn = "Text" }, null, query);
        }
    }
}
