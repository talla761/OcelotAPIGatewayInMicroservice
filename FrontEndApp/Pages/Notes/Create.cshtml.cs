using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Net.Http.Json;
using FrontEndApp.Models;

namespace FrontEndApp.Pages.Notes
{
    public class Create : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public Create(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [BindProperty]
        public Note Note { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token)) return RedirectToPage("/Account/Login");

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsJsonAsync(_configuration["Services:GatewayBaseUrl"] + "/api/notes", Note);

            if (response.IsSuccessStatusCode)
                return RedirectToPage("Index");

            ModelState.AddModelError(string.Empty, "Erreur lors de l'enregistrement de la note.");
            return Page();
        }
    }
}
