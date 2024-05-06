using Edu.Abstraction.Models;
using Microsoft.AspNetCore.Http;
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
    public class Intake : BaseClass
    {
        [Key]
        public long IntakeId { get; set; }

        [Required(ErrorMessage = "Please enater intake name")]
        public string IntakeName { get; set; }

        [Required(ErrorMessage = "Please select scheme")]
        [RegularExpression("(.*[1-9].*)|(.*[.].*[1-9].*)", ErrorMessage = "Please select scheme")]
        public long SchemeId { get; set; }

        [Required(ErrorMessage = "Please enter start date")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Please enter exam date")]
        public DateTime ExamDate { get; set; }

        [Required(ErrorMessage = "Please enter nomination cut off date")]
        public DateTime NominationCutoffDate { get; set; }

        [Required(ErrorMessage = "Please enter scorecard cut off date")]
        public DateTime ScorecardCutoffDate { get; set; }

        public bool? IsGTSScoreUpload { get; set; }

        public string BrochureFilePath { get; set; }

        public string BrochureFileName { get; set; }

        public virtual IList<IntakeLocation> Locations { get; set; } = new List<IntakeLocation>();

        [JsonIgnoreResponse]
        [JsonIgnoreRequest]
        [NotMapped]
        public string SelectedLocations{ get; set; }

        public virtual IList<IntakeDocumentType> DocumentTypes { get; set; } = new List<IntakeDocumentType>();

        [JsonIgnoreResponse]
        [JsonIgnoreRequest]
        [NotMapped]
        public string SelectedDocumentTypes { get; set; }

        public virtual IList<IntakeInstitute> Institutes { get; set; } = new List<IntakeInstitute>();

        public virtual IList<IntakeTemplate> Templates { get; set; } = new List<IntakeTemplate>();

        [JsonIgnoreResponse]
        [JsonIgnoreRequest]
        [NotMapped]
        public string SelectedInstitutes { get; set; }

        [NotMapped]
        [JsonIgnoreResponse]
        public IFormFile Brochure { get; set; }
    }
}
