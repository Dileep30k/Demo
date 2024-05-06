using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edu.Abstraction.ComplexModels
{
    public class ScorecardModel
    {
        public long NominationId { get; set; }
        public long MsilUserId { get; set; }
        public long UserId { get; set; }
        public decimal Score { get; set; }
        public string FileName { get; set; }
    }
}
