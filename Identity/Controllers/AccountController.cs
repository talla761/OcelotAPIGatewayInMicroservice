using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IdentityApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IdentityApi.Repositories.Interfaces;
using Microsoft.AspNetCore.DataProtection;

namespace IdentityApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IJwtAuthenticationRepository _jwtAuthenticationRepository;
        private readonly IConfiguration _config;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(IJwtAuthenticationRepository JwtAuthenticationRepository, IConfiguration config, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _jwtAuthenticationRepository = JwtAuthenticationRepository;
            _config = config;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        //api/account/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest("Email et mot de passe sont requis.");
            }

            // Récupérer l'utilisateur depuis AspNetUsers
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized("Utilisateur non trouvé.");
            }

            // Vérifier le mot de passe avec Identity
            bool isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid)
            {
                return Unauthorized("Mot de passe incorrect.");
            }

            if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Email))
            {
                return StatusCode(500, "Le compte utilisateur est invalide (Email ou UserName manquant).");
            }

            // Construire les revendications (claims)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Lire les paramètres JWT depuis la configuration
            var secret = _config["Jwt:Key"];
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(secret) || string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
            {
                throw new InvalidOperationException("Les paramètres JWT (Key, Issuer, Audience) sont manquants dans la configuration.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Durée de validité
            var expires = DateTime.UtcNow.AddMinutes(60);

            // Génération du token JWT complet
            var tokenDescriptor = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            // Sérialiser le token
            var jwt = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            // Retourner le token JWT
            return Ok(new
            {
                token = jwt,
                expiration = expires,
                user = new { user.Id, user.Email, user.UserName }
            });
        }

        //api/account/register/{email}/{password}
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
                return Ok("Utilisateur créé !");
            return BadRequest(result.Errors);
        }
    }
} 

