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
    public class NominationInstitute : BaseClass
    {
        [Key]
        public long NominationInstituteId { get; set; }

        public long NominationId { get; set; }

        public long InstituteId { get; set; }

        public bool? IsExamTaken { get; set; }
        public decimal? Rank { get; set; }

        public decimal? Score { get; set; }

        [JsonIgnoreResponse]
        [JsonIgnoreRequest]
        [NotMapped]
        [ForeignKey(nameof(NominationId))]
        public virtual Nomination Nomination { get; set; }

        [JsonIgnoreResponse]
        [JsonIgnoreRequest]
        [NotMapped]
        [ForeignKey(nameof(InstituteId))]
        public virtual Institute Institute { get; set; }
    }
}
