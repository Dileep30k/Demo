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
    public class IntakeTemplate : BaseClass
    {
        [Key]
        public long IntakeTemplateId { get; set; }

        public long IntakeId { get; set; }

        public long TemplateId { get; set; }

        [Required(ErrorMessage = "Please enter subject")]
        public string TemplateSubject { get; set; }

        [Required(ErrorMessage = "Please enter body")]
        public string TemplateContent { get; set; }

        [JsonIgnoreResponse]
        [JsonIgnoreRequest]
        [NotMapped]
        [ForeignKey(nameof(TemplateId))]
        public Template Template { get; set; }

        [JsonIgnoreResponse]
        [JsonIgnoreRequest]
        [NotMapped]
        [ForeignKey(nameof(IntakeId))]
        public Intake Intake { get; set; }
    }
}
