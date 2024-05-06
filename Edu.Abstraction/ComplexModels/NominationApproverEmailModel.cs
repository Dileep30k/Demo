using Edu.Abstraction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edu.Abstraction.ComplexModels
{
    public class NominationApproverEmailModel 
    {
        public long IntakeId { get; set; }
        public long MsilUserId { get; set; }
        public string StaffName { get; set; }
        public string StaffEmail { get; set; }
        public string MobileNo { get; set; }
        public string SchemeName { get; set; }
        public string IntakeName { get; set; }
        public string Approver1Name { get; set; }
        public string Approver2Name { get; set; }
        public string ApproverEmail { get; set; }
    }
}
