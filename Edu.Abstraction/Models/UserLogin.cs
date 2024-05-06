using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonIgnoreRequest = System.Text.Json.Serialization.JsonIgnoreAttribute;
using JsonIgnoreResponse = Newtonsoft.Json.JsonIgnoreAttribute;

namespace Edu.Abstraction.Models
{
    public class UserLogin : BaseClass
    {
        [Key]
        public long UserLoginId { get; set; }

        public long MsilUserId { get; set; }

        public string SessionId { get; set; }

        public DateTime LogInExpireTime { get; set; }
    }
}
