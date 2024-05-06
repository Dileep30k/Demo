using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edu.Abstraction.ComplexModels
{
    public class InstituteModel
    {
        public long InstituteId { get; set; }

        public string InstituteName { get; set; }

        public string EmailAddress { get; set; }

        public string ContactNo { get; set; }

        public string ContactPerson { get; set; }

        public string City { get; set; }
        public List<string> Schemes { get; set; }
        public List<IntakeInstituteSeatModel> Intakes { get; set; }
    }
    public class IntakeInstituteSeatModel
    {
        public string IntakeName { get; set; }
        public int TotalSeats { get; set; }
    }

}
