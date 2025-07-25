using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FrontEndApp.Models;

namespace FrontEndApp.Pages.Notes
{
    public class Index : PageModel
    {
        private readonly IHttpClientFactory _factory;
        private readonly IConfiguration _cfg;

        public Index(IHttpClientFactory factory, IConfiguration cfg)
        {
            _factory = factory;
            _cfg = cfg;
        }

        public List<Note> Notes { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            // Vérifie qu’on est authentifié
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToPage("/Account/Login");

            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var gateway = _cfg["Services:GatewayBaseUrl"];
            var resp = await client.GetAsync($"{gateway}/api/notes");

            if (!resp.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Impossible de charger les notes");
                return Page();
            }

            Notes = await resp.Content.ReadFromJsonAsync<List<Note>>() ?? new();
            return Page();
        }
    }
}
