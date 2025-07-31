using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FrontEndApp.Dtos;

namespace FrontEndApp.Pages.Patients
{
    public class Details : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public Details(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public PatientDto? Patient { get; set; }
        public List<NoteDto>? Notes { get; set; }

        public string? RiskLevel { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("Account/Login");

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var gateway = _configuration["Services:GatewayBaseUrl"];

            // 1. Get Patient
            var response = await client.GetAsync($"{gateway}/api/patient/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            Patient = await response.Content.ReadFromJsonAsync<PatientDto>();

            // 2. Get Assessment
            var riskResponse = await client.GetAsync($"{gateway}/api/assessment/{id}");
            if (riskResponse.IsSuccessStatusCode)
                RiskLevel = await riskResponse.Content.ReadAsStringAsync();

            // 3. Get Notes
            var notesResponse = await client.GetAsync($"{gateway}/api/notes/patient/{id}");
            if (notesResponse.IsSuccessStatusCode)
            {
                Notes = await notesResponse.Content.ReadFromJsonAsync<List<NoteDto>>();
            }

            return Page();
        }
    }
}
