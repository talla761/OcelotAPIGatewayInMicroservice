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

            // Si l'utilisateur est authentifié, générer un JWT
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName), // Par exemple, ajouter un nom d'utilisateur comme revendication
                new Claim(ClaimTypes.Email, user.Email),   // Ajouter l'email comme revendication
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Ajouter un identifiant unique (JTI)
            };

            // Générer le token JWT avec la clé secrète
            var secret = _config["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(secret))
                throw new InvalidOperationException("La clé secrète JWT est manquante dans la configuration.");

            var token = _jwtAuthenticationRepository.GenerateToken(secret, claims);

            // Retourner le token JWT à l'utilisateur
            return Ok(new
            {
                token,
                expiration = DateTime.UtcNow.AddMinutes(60),
                user = new { user.Id, user.Email, user.UserName }
            });
        }

        //private string GenerateToken(string getEmail)
        //{
        //    var key = Encoding.UTF8.GetBytes(config["Authentication:Key"]!);
        //    var securityKey = new SymmetricSecurityKey(key);
        //    var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        //    var claims = new[] { new Claim(ClaimTypes.Email, getEmail!) };

        //    var token = new JwtSecurityToken(
        //        issuer: config["Authentication:Issuer"],
        //        audience: config["Authentication:Audience"],
        //        claims: claims,
        //        expires: null,
        //        signingCredentials: credential);

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}

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
