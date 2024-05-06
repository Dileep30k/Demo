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
    public class IntakeInstitute : BaseClass
    {
        [Key]
        public long IntakeInstituteId { get; set; }

        public long IntakeId { get; set; }

        [Required(ErrorMessage = "Please select institute")]
        [RegularExpression("(.*[1-9].*)|(.*[.].*[1-9].*)", ErrorMessage = "Please select institute")]
        public long InstituteId { get; set; }

        [Required(ErrorMessage = "Please enter ")]
        [RegularExpression("(.*[1-9].*)|(.*[.].*[1-9].*)", ErrorMessage = "Please enter valid total seats")]
        public int TotalSeats { get; set; }

        [Required(ErrorMessage = "Please enter admission cut off date")]
        public DateTime AdmissionCutoffDate { get; set; }

        [JsonIgnoreResponse]
        [JsonIgnoreRequest]
        [NotMapped]
        [ForeignKey(nameof(InstituteId))]
        public Institute Institute { get; set; }

        [JsonIgnoreResponse]
        [JsonIgnoreRequest]
        [NotMapped]
        [ForeignKey(nameof(IntakeId))]
        public Intake Intake { get; set; }

        [JsonIgnoreResponse]
        [JsonIgnoreRequest]
        [NotMapped]
        public string InstitueName { get; set; }

    }
}
