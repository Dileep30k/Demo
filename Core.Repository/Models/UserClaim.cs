using System;
using System.Collections.Generic;

namespace Core.Repository.Models
{
    public class UserClaim
    {
        public long UserId { get; set; }
        public long MsilUserId { get; set; }
        public bool IsAdmin { get; set; }
        public long? DepartmentId { get; set; }
        public List<long> RoleIds { get; set; } = new List<long>();
        public string Username { get; set; }
        public string RoleName { get; set; }
        public string SessionId { get; set; }
        public bool IsApprover { get; set; }
        public bool IsEmployee { get; set; }
        public bool IsGts { get; set; }
    }
}
