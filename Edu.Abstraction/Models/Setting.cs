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
    public class Setting : BaseClass
    {
        [Key]
        public long SettingId { get; set; }

        [Required(ErrorMessage = "Please enter name")]
        public string SettingName { get; set; }

        public string SettingKey { get; set; }

        public string SettingValue { get; set; }

        public string SettingDesc { get; set; }
    }
}
