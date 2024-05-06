using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edu.Abstraction.ComplexModels
{
    public class DropdownModel
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserDropdownModel : DropdownModel
    {
        public long? ManagerId { get; set; }
        public long MsilUserId { get; set; }
        public DateTime? Doj { get; set; }
        public long? VerticalId { get; set; }

        public long? DivisionId { get; set; }

        public long? DesignationId { get; set; }

        public long? DepartmentId { get; set; }

        public long? LocationId { get; set; }
    }

    public class NominationUserDropdownModel : DropdownModel
    {
        public long MsilUserId { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public long NominationId { get; set; }
        public long NominationInstituteId { get; set; }
        public long AdmissionUserId { get; set; }
        public long SchemeId { get; set; }
        public long IntakeId { get; set; }
    }
}
