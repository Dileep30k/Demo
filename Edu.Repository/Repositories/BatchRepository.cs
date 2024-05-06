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
using Microsoft.OpenApi.Writers;
using Edu.Abstraction.Enums;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace Edu.Repository.Repositories
{
    public class BatchRepository : Repository<Batch>, IBatchRepository
    {
        public BatchRepository(EduDbContext context)
            : base(context)
        { }

        private EduDbContext EduDbContext
        {
            get { return _dbContext as EduDbContext; }
        }

        public async Task<PagedList> GetBatchPaged(Pagination pagination)
        {
            long? schemeId = IsPropertyExist(pagination.Filters, "schemeId") ? pagination.Filters?.schemeId : null;
            long? intakeId = IsPropertyExist(pagination.Filters, "intakeId") ? pagination.Filters?.intakeId : null;
            long? instituteId = IsPropertyExist(pagination.Filters, "instituteId") ? pagination.Filters?.instituteId : null;

            IRepository<BatchModel> repository = new Repository<BatchModel>(EduDbContext);
            var query = (from batch in EduDbContext.Batches
                         join intake in EduDbContext.Intakes on batch.IntakeId equals intake.IntakeId
                         join scheme in EduDbContext.Schemes on batch.SchemeId equals scheme.SchemeId
                         join institute in EduDbContext.Institutes on batch.InstituteId equals institute.InstituteId
                         where !batch.IsDeleted
                         && (!schemeId.HasValue || schemeId.Value == batch.SchemeId)
                         && (!intakeId.HasValue || intakeId.Value == batch.IntakeId)
                         && (!instituteId.HasValue || instituteId.Value == batch.InstituteId)
                         select new BatchModel
                         {
                             BatchId = batch.BatchId,
                             BatchCode = batch.BatchCode,
                             StartDate = batch.StartDate,
                             TotalSeats = batch.TotalSeats,
                             TotalFee = batch.TotalFee,
                             SchemeName = scheme.SchemeName,
                             IntakeName = intake.IntakeName,
                             InstituteName = institute.InstituteName,
                             AcademicYears = scheme.DurationTypeId == 1 ?
                             $"{intake.StartDate:yyyy}-{intake.StartDate.AddYears(scheme.Duration):yyyy}" :
                             $"{intake.StartDate:MMM yyyy}-{intake.StartDate.AddMonths(scheme.Duration):MMM yyyy}"
                         });

            return await repository.GetPagedReponseAsync(pagination, null, query);
        }

        public async Task<PagedList> GetDropdwon(long? id = null, long? schemeId = null, long? intakeId = null, long? instituteId = null)
        {
            Repository<DropdownModel> repository = new(EduDbContext);
            var query = (from r in EduDbContext.Batches
                         where !r.IsDeleted &&
                         (!id.HasValue || r.BatchId == id) &&
                         (!schemeId.HasValue || r.SchemeId == schemeId) &&
                         (!intakeId.HasValue || r.IntakeId == intakeId) &&
                         (!instituteId.HasValue || r.InstituteId == instituteId)
                         select new DropdownModel()
                         {
                             Id = r.BatchId,
                             Text = r.BatchCode,
                             IsActive = r.IsActive
                         });

            return await repository.GetPagedReponseAsync(new Pagination { SortOrderColumn = "Text" }, null, query);
        }

        public async Task<int> GetBatchCount(Batch batch)
        {
            return await EduDbContext.Batches.CountAsync(b => b.SchemeId == batch.SchemeId && b.InstituteId == batch.InstituteId);
        }

    }
}
