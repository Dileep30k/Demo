using Edu.Abstraction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edu.Abstraction.ComplexModels
{
    public class NominationModel : EligibilityModel
    {
        public string SchemeName { get; set; }
        public string StaffEmail { get; set; }
        public string MobileNo { get; set; }
        public long? Approver1StaffId { get; set; }
        public long? Approver2StaffId { get; set; }
        public bool IsEdit { get; set; }
        public DateTime? ExamDate { get; set; }
        public DateTime? IntakeStartDate { get; set; }
        public List<DocumentListModel> Documents { get; set; } = new List<DocumentListModel>();
        public List<string> InstituteNames { get; set; } = new List<string>();
        public List<string> Remarks { get; set; } = new List<string>();
    }

    public class DocumentListModel 
    {
        public string FileName { get; set; }

        public string FilePath { get; set; }
    }
}
