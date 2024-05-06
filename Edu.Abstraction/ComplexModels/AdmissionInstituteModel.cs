using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edu.Abstraction.ComplexModels
{
    public class AdmissionInstituteModel
    {
        public long? AdmissionUserId { get; set; }
        public long? AdmissionStatusId { get; set; }
        public string InstituteName { get; set; }
        public int? Rank { get; set; }
        public DateTime? AdmissionCutoffDate { get; set; }
        public bool? IsConfirmByEmp { get; set; }
        public bool? IsBondAccepted { get; set; }
        public DateTime? BondAcceptedDate { get; set; }
    }
}
