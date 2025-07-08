using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using PatientModel = Patient.Models.Patient;

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

        public PatientModel? Patient { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Account/Login");

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var gateway = _configuration["Services:GatewayBaseUrl"];
            var response = await client.GetAsync($"{gateway}/api/patient/{id}");

            if (response.IsSuccessStatusCode)
            {
                Patient = await response.Content.ReadFromJsonAsync<PatientModel>();
                return Page();
            }

            return NotFound();
        }
    }
}
