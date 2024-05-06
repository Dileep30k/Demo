using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edu.Abstraction.ComplexModels
{
    public class IntakeInstituteModel
    {
        public long IntakeInstituteId { get; set; }

        public string SchemeName { get; set; }
        public string InstituteName { get; set; }

        public int? TotalSeats { get; set; }

        public DateTime? AdmissionCutoffDate { get; set; }
        public DateTime NominationCutoffDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExamDate { get; set; }
        public List<string> EligibilityCriteria { get; set; } = new List<string>();
    }
}
