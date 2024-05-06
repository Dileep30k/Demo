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
    public class NominationStatus : BaseClass
    {
        [Key]
        public long NominationStatusId { get; set; }

        [Required(ErrorMessage = "Please enter name")]
        public string NominationStatusName { get; set; }
    }
}
