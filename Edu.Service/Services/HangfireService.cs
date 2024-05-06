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
using Core.Utility.Filters;
using Microsoft.Extensions.Logging;

namespace Edu.Service.Services
{
    public class HangfireService : IHangfireService
    {
        private readonly ILogger<HangfireService> _logger;
        private readonly INominationService _nominationService;

        public HangfireService(
           ILogger<HangfireService> logger,
           INominationService nominationService
        )
        {
            _nominationService = nominationService;
            _logger = logger;
        }

        public async Task PendingScorecardReminderEmails()
        {
            await _nominationService.PendingScorecardReminderEmails();
        }

        public async Task NominationApprover1ReminderEmails()
        {
            await _nominationService.NominationApprover1ReminderEmails();
        }
    }
}
