using Edu.Abstraction.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonIgnoreRequest = System.Text.Json.Serialization.JsonIgnoreAttribute;
using JsonIgnoreResponse = Newtonsoft.Json.JsonIgnoreAttribute;

namespace Edu.Abstraction.Models
{
    public class AdmissionUser : BaseClass
    {
        [Key]
        public long AdmissionUserId { get; set; }
        public long AdmissionId { get; set; }
        public long UserId { get; set; }
        public long NominationId { get; set; }
        public long NominationInstituteId { get; set; }
        public long AdmissionStatusId { get; set; }
        public int Rank { get; set; }
        public bool? IsConfirmByEmp { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public string EmployeeRemarks { get; set; }
        public bool? IsConfirmByInstitute { get; set; }
        public string ApproverRemarks { get; set; }
        public string Semester { get; set; }
        public bool? IsBondAccepted { get; set; }
        public DateTime? BondAcceptedDate { get; set; }

        [JsonIgnoreResponse]
        [JsonIgnoreRequest]
        [ForeignKey(nameof(AdmissionId))]
        public virtual Admission Admission { get; set; }
    }
}
