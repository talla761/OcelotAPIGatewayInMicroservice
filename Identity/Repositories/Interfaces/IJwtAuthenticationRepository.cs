using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IdentityApi.Repositories.Interfaces
{
    public interface IJwtAuthenticationRepository
    {
        //Task<IdentityUser> Authenticate(string email, string password);

        string GenerateToken(string secret, List<Claim> claims);
    }
}
