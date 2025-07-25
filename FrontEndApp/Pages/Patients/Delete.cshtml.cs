using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FrontEndApp.Models;

namespace FrontEndApp.Pages.Patients
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
        public Patient? Patient { get; set; }

        // GET  ? affiche la fiche et demande confirmation
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToPage("/Account/Login");

            var client = _httpFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var gateway = _config["Services:GatewayBaseUrl"];
            var resp = await client.GetAsync($"{gateway}/api/patient/{id}");

            if (!resp.IsSuccessStatusCode)
                return NotFound();

            Patient = await resp.Content.ReadFromJsonAsync<Patient>();
            return Page();
        }

        // POST ? supprime puis redirige vers Index
        public async Task<IActionResult> OnPostAsync()
        {
            if (Patient is null) return NotFound();

            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToPage("/Account/Login");

            var client = _httpFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var gateway = _config["Services:GatewayBaseUrl"];
            var resp = await client.DeleteAsync($"{gateway}/api/patient/{Patient.PatientId}");

            if (resp.IsSuccessStatusCode)
                return RedirectToPage("./Index");

            ModelState.AddModelError(string.Empty, "Erreur lors de la suppression");
            return Page();
        }
    }
}
