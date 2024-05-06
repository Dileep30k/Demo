using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edu.Abstraction.ComplexModels
{
    public class NominationFormModel
    {
        public long IntakeId { get; set; }
        public long NominationId { get; set; }

        [Required(ErrorMessage = "Please enter mobile no")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Please enter valid mobile no")]
        [StringLength(10, ErrorMessage = "Please enter valid mobile no", MinimumLength = 10)]
        [MaxLength(10)]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = "Please select gender")]
        public string Gender { get; set; }

        public string SelectedInstitutes { get; set; }

        public long MsilUserId { get; set; }
        public string StaffName { get; set; }
        public string Division { get; set; }
        public string Department { get; set; }
        public DateTime ExamDate { get; set; }
        public DateTime NominationCutoffDate { get; set; }
        public string NominationStatusName { get; set; }
        public long NominationStatusId { get; set; }

        [Range(0.0, 100, ErrorMessage = "Score must max 100")]
        public decimal? Score { get; set; }
        public bool? IsExamTaken { get; set; }

        public string SchemeName { get; set; }
    }
}
