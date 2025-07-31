using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Net.Http.Json;
using FrontEndApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FrontEndApp.Pages.Notes
{
    public class Create : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public List<SelectListItem> PatientsSelectList { get; set; } = new();

        public Create(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [BindProperty]
        public Note Note { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            await ChargerListePatients();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await ChargerListePatients();
                return Page();
            }

            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Account/Login");

            // ?? Ajout de la date ici
            Note.CreatedAt = DateTime.Now;

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsJsonAsync($"{_configuration["Services:GatewayBaseUrl"]}/api/notes", Note);

            if (response.IsSuccessStatusCode)
                return RedirectToPage("Index");

            ModelState.AddModelError(string.Empty, "Erreur lors de l'enregistrement de la note.");
            await ChargerListePatients();
            return Page();
        }

        private async Task ChargerListePatients()
        {
            var httpClient = _httpClientFactory.CreateClient();

            // Ajout du jeton si l'API des patients est protégée
            var token = HttpContext.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var endpoint = $"{_configuration["Services:GatewayBaseUrl"]}/api/patient";
            var response = await httpClient.GetAsync(endpoint);

            System.Diagnostics.Debug.WriteLine($"Appel GET {endpoint} => {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine("Réponse JSON brute : " + json);

                var patients = JsonSerializer.Deserialize<List<PatientDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (patients != null && patients.Any())
                {
                    PatientsSelectList = patients.Select(p => new SelectListItem
                    {
                        Value = p.PatientId.ToString(),
                        Text = $"{p.Prenom} {p.Nom}"
                    }).ToList();

                    System.Diagnostics.Debug.WriteLine($"Patients chargés : {PatientsSelectList.Count}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("?? Aucun patient désérialisé.");
                    PatientsSelectList = new();
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("? Échec de récupération des patients");
                PatientsSelectList = new();
            }
        }
    }
}
