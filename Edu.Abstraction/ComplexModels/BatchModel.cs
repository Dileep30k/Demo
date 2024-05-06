using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edu.Abstraction.ComplexModels
{
    public class BatchModel
    {
        public long BatchId { get; set; }
        public string BatchCode { get; set; }
        public string SchemeName { get; set; }
        public string IntakeName { get; set; }
        public string InstituteName { get; set; }
        public DateTime StartDate { get; set; }

        public string AcademicYears { get; set; }
        public long TotalSeats { get; set; }

        public long TotalFee { get; set; }
    }
}
