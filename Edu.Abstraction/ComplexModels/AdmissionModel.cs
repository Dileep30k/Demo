using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edu.Abstraction.ComplexModels
{
    public class AdmissionModel
    {
        public long AdmissionId { get; set; }
        public long SchemeId { get; set; }
        public long IntakeId { get; set; }
        public long InstituteId { get; set; }
        public long? ApprovalBy1 { get; set; }
        public long? ApprovalBy2 { get; set; }
    }
}
