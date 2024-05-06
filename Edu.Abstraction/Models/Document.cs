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
    public class Document : BaseClass
    {
        [Key]
        public long DocumentId { get; set; }

        public long DocumentTableId { get; set; }

        public string DocumentTable { get; set; }

        public string DocumentType { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public string ContentType { get; set; }

        [NotMapped]
        [JsonIgnoreResponse]
        public IFormFile DocumentFile { get; set; }
    }
}
