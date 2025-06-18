using IdentityApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityApi.Repositories
{
    public class JwtAuthenticationRepository : IJwtAuthenticationRepository
    {
        private readonly UserManager<IdentityUser> _userManager;

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        public JwtAuthenticationRepository(UserManager<IdentityUser> userManager,
                       SignInManager<IdentityUser> signInManager,
                       IConfiguration configuration)
        {
            _userManager = userManager;

            _signInManager = signInManager;
            _configuration = configuration;
        }

        //public async Task<IdentityUser> Authenticate(string email, string password)
        //{
        //    // Récupérer l'utilisateur par email
        //    var user = await _userManager.FindByEmailAsync(email);
        //    if (user == null)
        //    {
        //        return null; // Utilisateur non trouvé
        //    }

        //    // Vérifier le mot de passe
        //    var isPasswordValid = await _signInManager.PasswordSignInAsync(user, password, false, false);  //await _userManager.CheckPasswordAsync(user, password);
        //    if (isPasswordValid.Succeeded)
        //    {
        //        return user;
        //    }

        //    return null; // Mot de passe invalide
        //}

        public string GenerateToken(string secret, List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
