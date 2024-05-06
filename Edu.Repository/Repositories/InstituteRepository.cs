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
    public class InstituteRepository : Repository<Institute>, IInstituteRepository
    {
        public InstituteRepository(EduDbContext context)
            : base(context)
        { }

        private EduDbContext EduDbContext
        {
            get { return _dbContext as EduDbContext; }
        }

        public async Task<PagedList> GetInstitutePaged(Pagination pagination)
        {
            string searchValue = IsPropertyExist(pagination.Filters, "searchValue") ? pagination.Filters?.searchValue : null;
            long? schemeId = IsPropertyExist(pagination.Filters, "schemeId") ? pagination.Filters?.schemeId : null;

            IRepository<InstituteModel> repository = new Repository<InstituteModel>(EduDbContext);
            var query = (from ins in EduDbContext.Institutes
                         where !ins.IsDeleted
                         && (string.IsNullOrEmpty(searchValue) || ins.InstituteName.Contains(searchValue))
                         && (!schemeId.HasValue || EduDbContext.SchemeInstitutes
                                      .Where(si => si.InstituteId == ins.InstituteId && schemeId.Value == si.SchemeId)
                                      .Select(i => i.InstituteId).Contains(ins.InstituteId))
                         select new InstituteModel()
                         {
                             InstituteId = ins.InstituteId,
                             InstituteName = ins.InstituteName,
                             EmailAddress = ins.EmailAddress,
                             ContactNo = ins.ContactNo,
                             ContactPerson = ins.ContactPerson,
                             City = ins.City,
                             Schemes = (from si in EduDbContext.SchemeInstitutes
                                        join scheme in EduDbContext.Schemes on si.SchemeId equals scheme.SchemeId
                                        where si.InstituteId == ins.InstituteId
                                        select scheme.SchemeName).ToList(),
                             Intakes = (from ii in EduDbContext.IntakeInstitutes
                                        join intake in EduDbContext.Intakes on ii.IntakeId equals intake.IntakeId
                                        where ii.InstituteId == ins.InstituteId
                                        select new IntakeInstituteSeatModel { IntakeName = intake.IntakeName, TotalSeats = ii.TotalSeats }).ToList()
                         });

            return await repository.GetPagedReponseAsync(pagination, null, query);
        }

        public async Task<PagedList> GetDropdwon(long? id = null, bool? isActive = null)
        {
            Repository<DropdownModel> repository = new(EduDbContext);
            var query = (from r in EduDbContext.Institutes
                         where !r.IsDeleted &&
                         (!id.HasValue || r.InstituteId == id) &&
                         (!isActive.HasValue || r.IsActive == isActive)
                         select new DropdownModel()
                         {
                             Id = r.InstituteId,
                             Text = r.InstituteName,
                             IsActive = r.IsActive
                         });

            return await repository.GetPagedReponseAsync(new Pagination { SortOrderColumn = "Text" }, null, query);
        }
    }
}
