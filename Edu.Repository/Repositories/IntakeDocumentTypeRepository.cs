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
    public class IntakeDocumentTypeRepository : Repository<IntakeDocumentType>, IIntakeDocumentTypeRepository
    {
        public IntakeDocumentTypeRepository(EduDbContext context)
            : base(context)
        { }

        private EduDbContext EduDbContext
        {
            get { return _dbContext as EduDbContext; }
        }

        public async Task<PagedList> GetIntakeDocumentTypePaged(Pagination pagination)
        {
            long? intakeId = IsPropertyExist(pagination.Filters, "intakeId") ? pagination.Filters?.intakeId : null;

            IRepository<IntakeDocumentTypeModel> repository = new Repository<IntakeDocumentTypeModel>(EduDbContext);
            var query = (from r in EduDbContext.IntakeDocumentTypes
                         join doc in EduDbContext.DocumentTypes on r.DocumentTypeId equals doc.DocumentTypeId
                         where !r.IsDeleted
                         && (!intakeId.HasValue || intakeId.Value == r.IntakeId)
                         select new IntakeDocumentTypeModel()
                         {
                             IntakeDocumentTypeId = r.IntakeDocumentTypeId,
                             DocumentTypeName = doc.DocumentTypeName
                         });

            return await repository.GetPagedReponseAsync(pagination, null, query);
        }

        public async Task<PagedList> GetDropdwon(long? id = null, bool? isActive = null)
        {
            Repository<DropdownModel> repository = new(EduDbContext);
            var query = (from r in EduDbContext.IntakeDocumentTypes
                         where !r.IsDeleted &&
                         (!id.HasValue || r.IntakeDocumentTypeId == id) &&
                         (!isActive.HasValue || r.IsActive == isActive)
                         select new DropdownModel()
                         {
                             Id = r.IntakeDocumentTypeId,
                             IsActive = r.IsActive
                         });

            return await repository.GetPagedReponseAsync(new Pagination { SortOrderColumn = "Text" }, null, query);
        }
    }
}
