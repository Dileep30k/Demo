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
    public class User : BaseClass
    {
        [Key]
        public long UserId { get; set; }

        [EmailAddress(ErrorMessage = "Please enter valid email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter first name")]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [RegularExpression("^[0-9]*$", ErrorMessage = "Please enter valid mobile no")]
        [StringLength(10, ErrorMessage = "Please enter valid mobile no", MinimumLength = 10)]
        [MaxLength(10)]
        public string MobileNo { get; set; }

        [RegularExpression("(.*[1-9].*)|(.*[.].*[1-9].*)", ErrorMessage = "Please select manager")]
        public long? ManagerId { get; set; }

        [Required(ErrorMessage = "Please enter msil user id")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Please enter valid msil user id")]
        public long MsilUserId { get; set; }

        public string MsilUserName { get; set; }

        public DateTime? Doj { get; set; }
        public DateTime? Dos { get; set; }
        public DateTime? Dol { get; set; }

        public string Password { get; set; }

        public string Salt { get; set; }

        [RegularExpression("(.*[1-9].*)|(.*[.].*[1-9].*)", ErrorMessage = "Please select vertical")]
        public long? VerticalId { get; set; }

        [RegularExpression("(.*[1-9].*)|(.*[.].*[1-9].*)", ErrorMessage = "Please select division")]
        public long? DivisionId { get; set; }

        [RegularExpression("(.*[1-9].*)|(.*[.].*[1-9].*)", ErrorMessage = "Please select designation")]
        public long? DesignationId { get; set; }

        [RegularExpression("(.*[1-9].*)|(.*[.].*[1-9].*)", ErrorMessage = "Please select department")]
        public long? DepartmentId { get; set; }

        [RegularExpression("(.*[1-9].*)|(.*[.].*[1-9].*)", ErrorMessage = "Please select location")]
        public long? LocationId { get; set; }

        public bool IsAdmin { get; set; }
    }
}
