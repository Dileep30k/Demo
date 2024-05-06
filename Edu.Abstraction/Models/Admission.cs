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
    public class Admission : BaseClass
    {
        [Key]
        public long AdmissionId { get; set; }

        public long SchemeId { get; set; }

        public long IntakeId { get; set; }

        public long InstituteId { get; set; }

        public long? ApprovalBy1 { get; set; }

        public bool? Approved1 { get; set; }

        public DateTime? ApprovalDate1 { get; set; }

        public string ApprovalRemarks1 { get; set; }

        public long? ApprovalBy2 { get; set; }

        public bool? Approved2 { get; set; }

        public DateTime? ApprovalDate2 { get; set; }

        public string ApprovalRemarks2 { get; set; }

        public bool IsPublish { get; set; }

        public bool IsConfirmUpload { get; set; }

        public virtual IList<AdmissionUser> Users { get; set; } = new List<AdmissionUser>();

        [JsonIgnoreRequest]
        [NotMapped]
        public bool? IsApprovedByUser { get; set; }

    }
}
