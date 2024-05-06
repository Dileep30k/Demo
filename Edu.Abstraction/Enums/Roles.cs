using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edu.Abstraction.Enums
{
    public enum Roles
    {
        [Description("GTS")]
        GTS = 1,

        [Description("Approver")]
        Approver = 2,

        [Description("Employee")]
        Employee = 3,
    }
}
