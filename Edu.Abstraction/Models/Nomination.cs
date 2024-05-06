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
    public class Nomination : BaseClass
    {
        [Key]
        public long NominationId { get; set; }

        public long UserId { get; set; }

        public long SchemeId { get; set; }

        public long IntakeId { get; set; }

        public long NominationStatusId { get; set; }

        [RegularExpression("^[0-9]*$", ErrorMessage = "Please enter valid mobile no")]
        [StringLength(10, ErrorMessage = "Please enter valid mobile no", MinimumLength = 10)]
        [MaxLength(10)]
        public string MobileNo { get; set; }

        public string Gender { get; set; }
        
        public decimal? MsilTenure { get; set; }

        public string Grade { get; set; }

        public decimal? RelevantPrevExp { get; set; }

        public long? VerticalId { get; set; }

        public long? DivisionId { get; set; }

        public long? DesignationId { get; set; }

        public long? DepartmentId { get; set; }

        public long? LocationId { get; set; }

        public long? InstitueId { get; set; }

        public long? ApprovalBy1 { get; set; }

        public DateTime? ApprovalDate1 { get; set; }

        public string ApprovalRemarks1 { get; set; }

        public long? ApprovalBy2 { get; set; }

        public DateTime? ApprovalDate2 { get; set; }

        public string ApprovalRemarks2 { get; set; }

        public long? AdmissionStatusId { get; set; }

        public DateTime? AdmissionAcceptedDate { get; set; }

        public bool? IsExamTaken { get; set; }

        public decimal? Rank { get; set; }

        public decimal? Score { get; set; }

        public bool IsScoreApprove { get; set; }

        public bool IsPublish { get; set; }

        public virtual IList<NominationInstitute> Institutes { get; set; } = new List<NominationInstitute>();
    }
}
