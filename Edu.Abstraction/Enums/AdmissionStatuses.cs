using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edu.Abstraction.Enums
{
    public enum AdmissionStatuses
    {
        [Description("Confirm")]
        Confirm = 1,

        [Description("Waiting")]
        Waiting = 2,

        [Description("Active")]
        Active = 3,

        [Description("Left")]
        Left = 4,

        [Description("Pause")]
        Pause = 5,

        [Description("Accepted")]
        Accepted = 6,

        [Description("Rejected")]
        Rejected = 7,
    }
}
