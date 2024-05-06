using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edu.Service.Interfaces
{
    public interface IHangfireService
    {
        Task PendingScorecardReminderEmails();
        Task NominationApprover1ReminderEmails();
    }
}
