using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FrontEndApp.Models;

namespace FrontEndApp.Pages.Notes
{
    public class Delete : PageModel
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly IConfiguration _config;

        public Delete(IHttpClientFactory httpFactory, IConfiguration config)
        {
            _httpFactory = httpFactory;
            _config = config;
        }

        [BindProperty]
        public Note? Note { get; set; }

        // GET : affiche la note à supprimer
        public async Task<IActionResult> OnGetAsync(string id)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToPage("/Account/Login");

            var client = _httpFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var gateway = _config["Services:GatewayBaseUrl"];
            var resp = await client.GetAsync($"{gateway}/api/notes/{id}");

            if (!resp.IsSuccessStatusCode)
                return NotFound();

            Note = await resp.Content.ReadFromJsonAsync<Note>();
            return Page();
        }

        // POST : déclenche la suppression puis redirige vers l’index
        public async Task<IActionResult> OnPostAsync()
        {
            if (Note == null) return NotFound();

            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToPage("/Account/Login");

            var client = _httpFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var gateway = _config["Services:GatewayBaseUrl"];
            var resp = await client.DeleteAsync($"{gateway}/api/notes/{Note.Id}");

            if (resp.IsSuccessStatusCode)
                return RedirectToPage("/Notes/Index");

            ModelState.AddModelError(string.Empty, "Erreur lors de la suppression.");
            return Page();
        }
    }
}
