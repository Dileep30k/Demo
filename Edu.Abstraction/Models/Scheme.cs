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
    public class Scheme : BaseClass
    {
        [Key]
        public long SchemeId { get; set; }

        [Required(ErrorMessage = "Please enter scheme name")]
        public string SchemeName { get; set; }

        [Required(ErrorMessage = "Please enter scheme code")]
        public string SchemeCode { get; set; }

        [Required(ErrorMessage = "Please enter duration")]
        [RegularExpression("(.*[1-9].*)|(.*[.].*[1-9].*)", ErrorMessage = "Please enter valid duration")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "Please select duration type")]
        [RegularExpression("(.*[1-9].*)|(.*[.].*[1-9].*)", ErrorMessage = "Please select duration type")]
        public long DurationTypeId { get; set; }

        public virtual IList<SchemeInstitute> Institutes { get; set; } = new List<SchemeInstitute>();   

        [JsonIgnoreResponse]
        [JsonIgnoreRequest]
        [NotMapped]
        public string SelectedInstitutes { get; set; }
    }
}
