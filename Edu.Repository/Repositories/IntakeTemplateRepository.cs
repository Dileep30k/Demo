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
using Microsoft.EntityFrameworkCore;

namespace Edu.Repository.Repositories
{
    public class IntakeTemplateRepository : Repository<IntakeTemplate>, IIntakeTemplateRepository
    {
        public IntakeTemplateRepository(EduDbContext context)
            : base(context)
        { }

        private EduDbContext EduDbContext
        {
            get { return _dbContext as EduDbContext; }
        }

        public async Task<PagedList> GetIntakeTemplatePaged(Pagination pagination)
        {
            long? schemeId = IsPropertyExist(pagination.Filters, "schemeId") ? pagination.Filters?.schemeId : null;
            long? intakeId = IsPropertyExist(pagination.Filters, "intakeId") ? pagination.Filters?.intakeId : null;

            IRepository<IntakeTemplateModel> repository = new Repository<IntakeTemplateModel>(EduDbContext);
            var query = (from bi in EduDbContext.IntakeTemplates
                         join ins in EduDbContext.Templates on bi.TemplateId equals ins.TemplateId
                         join intake in EduDbContext.Intakes on bi.IntakeId equals intake.IntakeId
                         where !bi.IsDeleted
                         && (!schemeId.HasValue || schemeId.Value == intake.SchemeId)
                         && (!intakeId.HasValue || intakeId.Value == intake.IntakeId)
                         select new IntakeTemplateModel()
                         {
                             IntakeTemplateId = bi.IntakeTemplateId,
                             IntakeName = intake.IntakeName,
                             TemplateSubject = bi.TemplateSubject,
                             TemplateName = ins.TemplateName,
                         });

            return await repository.GetPagedReponseAsync(pagination, null, query);
        }

        public async Task<PagedList> GetDropdwon(long? id = null, bool? isActive = null)
        {
            Repository<DropdownModel> repository = new(EduDbContext);
            var query = (from r in EduDbContext.IntakeTemplates
                         join ins in EduDbContext.Templates on r.TemplateId equals ins.TemplateId
                         where !r.IsDeleted &&
                         (!id.HasValue || r.IntakeTemplateId == id) &&
                         (!isActive.HasValue || r.IsActive == isActive)
                         select new DropdownModel()
                         {
                             Id = r.IntakeTemplateId,
                             Text = ins.TemplateName,
                             IsActive = r.IsActive
                         });

            return await repository.GetPagedReponseAsync(new Pagination { SortOrderColumn = "Text" }, null, query);
        }

        public async Task<IntakeTemplate> GetIntakeTemplateByKey(long intakeId, string key)
        {
            return await (from it in EduDbContext.IntakeTemplates
                          join template in EduDbContext.Templates on it.TemplateId equals template.TemplateId
                          where !it.IsDeleted
                          && it.IntakeId == intakeId
                          && template.TemplateKey == key
                          select it).FirstOrDefaultAsync();
        }

        public async Task<IList<IntakeTemplate>> GetIntakeTemplatesByKey(List<long> intakeIds, string key)
        {
            return await (from it in EduDbContext.IntakeTemplates
                          join template in EduDbContext.Templates on it.TemplateId equals template.TemplateId
                          where !it.IsDeleted
                          && intakeIds.Contains(it.IntakeId)
                          && template.TemplateKey == key
                          select it).ToListAsync();
        }
    }
}
