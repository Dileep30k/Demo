using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edu.Abstraction.ComplexModels
{
    public class BatchUserModel
    {
        public long BatchId { get; set; }
        public long BatchUserId { get; set; }
        public long UserId { get; set; }
    }
}
