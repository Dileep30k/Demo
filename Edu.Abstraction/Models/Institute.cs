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
    public class Institute : BaseClass
    {
        [Key]
        public long InstituteId { get; set; }

        [Required(ErrorMessage = "Please enter institute name")]
        public string InstituteName { get; set; }

        [Required(ErrorMessage = "Please enter institute code")]
        public string InstituteCode { get; set; }

        [EmailAddress(ErrorMessage = "Please enter valid email")]
        public string EmailAddress { get; set; }

        [StringLength(10, ErrorMessage = "Please enter valid contact no", MinimumLength = 10)]
        [MaxLength(10)]
        public string ContactNo { get; set; }

        public string ContactPerson { get; set; }

        public string Address { get; set; }

        [StringLength(6, ErrorMessage = "Please enter valid pincode", MinimumLength = 6)]
        [MaxLength(6)]
        public string Pincode { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }
    }
}
