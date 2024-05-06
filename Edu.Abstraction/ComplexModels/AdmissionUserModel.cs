using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edu.Abstraction.ComplexModels
{
    public class AdmissionUserModel
    {
        public long AdmissionId { get; set; }
        public long AdmissionUserId { get; set; }
        public long NominationId { get; set; }
        public long NominationInstituteId { get; set; }
        public long SchemeId { get; set; }
        public long IntakeId { get; set; }
        public long InstituteId { get; set; }
        public long AdmissionStatusId { get; set; }
        public long UserId { get; set; }
        public int Rank { get; set; }
        public long MsilUserId { get; set; }
        public string StaffName { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public long? ApprovalBy1 { get; set; }
        public long? ApprovalBy2 { get; set; }
        public bool? IsConfirmByEmp { get; set; }
        public string EmployeeRemarks { get; set; }
        public bool? IsConfirmByInstitute { get; set; }
        public string ApproverRemarks { get; set; }
        public string Semester { get; set; }
        public string Status { get; set; }
        public DateTime? IntakeStartDate { get; set; }
        public List<DocumentListModel> Documents { get; set; } = new List<DocumentListModel>();

    }

    public class ConfirmAdmissionUserModel
    {
        public long AdmissionUserId { get; set; }
        public long AdmissionStatusId { get; set; }
        public long MsilUserId { get; set; }
        public string StaffName { get; set; }
        public string Status { get; set; }
        public string Semester { get; set; }
        public string Vertical { get; set; }
        public string Designation { get; set; }
        public string Division { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
        public int Rank { get; set; }
        public string ReportingManager { get; set; }
        public string Remarks { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public List<DocumentListModel> Documents { get; set; } = new List<DocumentListModel>();
    }
}
