using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edu.Abstraction.ComplexModels
{
    public class SettingModel
    {
        public long SettingId { get; set; }
        public string SettingName { get; set; }
        public string SettingValue { get; set; }
        public bool IsActive { get; set; }
    }
}
