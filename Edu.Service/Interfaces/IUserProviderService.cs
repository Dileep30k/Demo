using Core.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edu.Service.Interfaces
{
    public interface IUserProviderService
    {
        UserClaim UserClaim { get; }

        bool IsAuthenticated();
        void RemoveSessionUser();
    }
}
