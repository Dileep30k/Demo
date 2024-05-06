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
    public class Template : BaseClass
    {
        [Key]
        public long TemplateId { get; set; }

        public string TemplateName { get; set; }

        public string TemplateKey { get; set; }

        public string TemplateSubject { get; set; }
        public string TemplateContent { get; set; }

    }
}
