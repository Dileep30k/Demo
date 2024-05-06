using Core.Repository;
using Core.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edu.Abstraction.Models;

namespace Edu.Repository.Interfaces
{
    public interface IIntakeTemplateRepository : IRepository<IntakeTemplate>
    {
        Task<PagedList> GetIntakeTemplatePaged(Pagination pagination);
        Task<PagedList> GetDropdwon(long? id = null, bool? isActive = null);
        Task<IntakeTemplate> GetIntakeTemplateByKey(long intakeId, string key);
        Task<IList<IntakeTemplate>> GetIntakeTemplatesByKey(List<long> intakeIds, string key);
    }
}
