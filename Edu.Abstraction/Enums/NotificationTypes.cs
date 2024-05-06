using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edu.Abstraction.Enums
{
    public enum NotificationTypes
    {
        [Description("Info")]
        Info,

        [Description("Warning")]
        Warning,

        [Description("Success")]
        Success,

        [Description("Error")]
        Error,
    }
}
