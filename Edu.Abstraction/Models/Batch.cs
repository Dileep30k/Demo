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
    public class Batch : BaseClass
    {
        [Key]
        public long BatchId { get; set; }

        public string BatchCode { get; set; }

        public long SchemeId { get; set; }

        public long IntakeId { get; set; }

        public long InstituteId { get; set; }

        public long AdmissionId { get; set; }

        public DateTime StartDate { get; set; }

        public int TotalSeats { get; set; }

        public int TotalFee { get; set; }

        public virtual IList<BatchUser> Users { get; set; } = new List<BatchUser>();
    }
}
