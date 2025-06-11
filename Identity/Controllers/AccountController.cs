using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Identity.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController(IConfiguration config) : ControllerBase
    {
        private static ConcurrentDictionary<string, string> UserData { get; set; } = 
            new ConcurrentDictionary<string, string>();

        //api/account/login/{email}/{password}
        [HttpPost("/login/{email}/{password}")]
        public async Task<IActionResult> Login(string email, string password)
        {
            await Task.Delay(500);
            var getEmail = UserData!.Keys.Where(e => e.Equals(email)).FirstOrDefault();

            if (!string.IsNullOrEmpty(getEmail))
            {
                UserData.TryGetValue(getEmail, out string? dbPassword);
                if (!Equals(dbPassword, password))
                    return BadRequest("Invalid credentials");

                string jwtToken = GenerateToken(getEmail);
                return Ok(jwtToken);
            }
            return NotFound("Email not found");
        }

        private string GenerateToken(string getEmail)
        {
            var key = Encoding.UTF8.GetBytes(config["Authentication:Key"]!);
            var securityKey = new SymmetricSecurityKey(key);
            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[] { new Claim(ClaimTypes.Email, getEmail!) };

            var token = new JwtSecurityToken(
                issuer: config["Authentication:Issuer"],
                audience: config["Authentication:Audience"],
                claims: claims,
                expires: null,
                signingCredentials: credential);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
