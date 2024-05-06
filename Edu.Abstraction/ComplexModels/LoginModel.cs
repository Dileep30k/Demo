using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonIgnoreRequest = System.Text.Json.Serialization.JsonIgnoreAttribute;
using JsonIgnoreResponse = Newtonsoft.Json.JsonIgnoreAttribute;

namespace Edu.Abstraction.ComplexModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Please enter staff id")]
        public long MsilUserId { get; set; }

        [Required(ErrorMessage = "Please enter password")]
        public string Password { get; set; }

        [JsonIgnoreRequest]
        public string ReturnUrl { get; set; }

        public bool ForceLogin { get; set; }
    }
}
