using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FrontEndApp.Models;

namespace FrontEndApp.Pages.Patients
{
    public class Create : PageModel
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly IConfiguration _config;

        public Create(IHttpClientFactory httpFactory, IConfiguration config)
        {
            _httpFactory = httpFactory;
            _config = config;
        }

        [BindProperty]
        public Patient NewPatient { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Récupère le token stocké en session
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrWhiteSpace(token))
            {
                // Non authentifié ? renvoi vers la page de connexion
                return RedirectToPage("/Account/Login");
            }

            var client = _httpFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var gateway = _config["Services:GatewayBaseUrl"];
            var response = await client.PostAsJsonAsync($"{gateway}/api/patient", NewPatient);

            if (response.IsSuccessStatusCode)
            {
                // Retour à la liste des patients
                return RedirectToPage("./Index");
            }

            ModelState.AddModelError(string.Empty,
                $"Erreur API : {(int)response.StatusCode} {response.ReasonPhrase}");
            return Page();
        }
    }
}
