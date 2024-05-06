using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edu.Abstraction.Enums
{
    public enum SettingKeys
    {
        [Description("Reciepient List")]
        ReciepientList = 1,

        [Description("Override Email Address")]
        OverrideEmail = 2,
    }
}
