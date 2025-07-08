using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace FrontEndApp.Pages.Account
{
    public class Login : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public Login(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [BindProperty] public string Email { get; set; }
        [BindProperty] public string Password { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var loginData = new { Email, Password };
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync(_configuration["Services:GatewayBaseUrl"] + "/api/account/login", loginData);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);
                var token = doc.RootElement.GetProperty("token").GetString();

                HttpContext.Session.SetString("JwtToken", token!);
                return RedirectToPage("/Patients/Index");
            }
            else
            {
                ErrorMessage = "Échec de la connexion";
                return Page();
            }
        }
    }
}
