using Core.Utility.Utils;
using Edu.Abstraction.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edu.Abstraction.ComplexModels
{
    public class EligibilityModel : Nomination
    {
        public long MsilUserId { get; set; }
        public string StaffName { get; set; }
        public DateTime? Doj { get; set; }
        public string Vertical { get; set; }
        public string Designation { get; set; }
        public string Division { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
        public string Approver1 { get; set; }
        public string Approver2 { get; set; }
        public DateTime? IntakeStartDate { get; set; }
    }
}
