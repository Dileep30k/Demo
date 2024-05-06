using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edu.Abstraction.ComplexModels
{
    public class IntakeModel
    {
        public long IntakeId { get; set; }
        public string IntakeName { get; set; }
        public DateTime? StartDate { get; set; }
        public string SchemeName { get; set; }
        public DateTime? NominationCutoffDate { get; set; }
        public List<IntakeInstituteGridModel> Institutes { get; set; } = new List<IntakeInstituteGridModel>();
        public DateTime? ExamDate { get; set; }
        public List<string> DocumentTypes { get; set; } = new List<string>();
    }

    public class IntakeInstituteGridModel
    {
        public string InstituteName { get; set; }
        public string InstituteCode { get; set; }
        public int TotalSeats { get; set; }
        public DateTime AdmissionCutoffDate { get; set; }
    }
}
