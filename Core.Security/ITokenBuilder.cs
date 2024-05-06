using Core.Repository.Models;

namespace Core.Security
{
    public interface ITokenBuilder
    {
        string BuildToken(UserClaim userClaims);
    }
}
