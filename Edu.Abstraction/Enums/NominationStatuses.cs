using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edu.Abstraction.Enums
{
    public enum NominationStatuses
    {
        [Description("Nominated")]
        Nominated = 1,

        [Description("Submitted")]
        Submitted = 2,

        [Description("Accepted")]
        Accepted = 3,

        [Description("Rejected")]
        Rejected = 4,

        [Description("Department Approved")]
        DepApprove = 5,

        [Description("Department Reviewed")]
        DepReview = 6,

        [Description("Department Rejected")]
        DepRejected = 7,

        [Description("Division Approved")]
        DivApprove = 8,

        [Description("Division Reviewed")]
        DivReview = 9,

        [Description("Division Rejected")]
        DivRejected = 10,

        [Description("Approved")]
        Approved = 11,

        [Description("Waiting")]
        Waiting = 12,

        [Description("Confirmed")]
        Confirmed = 13,

    }
}
