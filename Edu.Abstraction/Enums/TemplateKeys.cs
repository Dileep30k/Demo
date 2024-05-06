using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edu.Abstraction.Enums
{
    public enum TemplateKeys
    {
        [Description("Eligibility Email")]
        EligibilityEmail = 1,

        [Description("Nomination Form")]
        NominationForm = 2,

        [Description("Admission Email")]
        AdmissionEmail = 3,

        [Description("Admission Confirm Email")]
        AdmissionConfirmEmail = 4,

        [Description("Scorecard Pending Email")]
        PendingScorecardEmail = 5,

        [Description("Nomination Approver 1 Email")]
        NominationApprover1 = 6,

        [Description("Nomination Approver 2 Email")]
        NominationApprover2 = 7,
    }

    public static class TemplateDataKeys
    {
        public static string SchemeName = "#SchemeName#";
        public static string IntakeName = "#IntakeName#";
        public static string InstituteName = "#InstituteName#";
        
        public static string InstituteList = "#InstituteList#";
        public static string InstituteNameSeatList = "#InstituteNameSeatList#";
        
        public static string Meritlist = "#Meritlist#";
        public static string Waitlist = "#Waitlist#";
        public static string ConfirmationList = "#ConfirmationList#";

        public static string IntakeStartDate = "#IntakeStartDate#";
        public static string ExamDate = "#ExamDate#";
        public static string NominationCutoffDate = "#NominationCutoffDate#";
        public static string ScoreCutoffDate = "#ScoreCutoffDate#";

        public static string NominationStaffList = "#NominationStaffList#";
        public static string NominationApprover1Name = "#Approver1Name#";
        public static string NominationApprover2Name = "#Approver2Name#";
    }
}
