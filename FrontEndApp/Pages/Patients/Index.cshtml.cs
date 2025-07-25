using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FrontEndApp.Models;

namespace FrontEndApp.Pages.Patients
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public List<Patient> Patients { get; set; } = new();

        public async Task OnGetAsync()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token)) return;

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync(_configuration["Services:GatewayBaseUrl"] + "/api/patient");
            if (response.IsSuccessStatusCode)
            {
                Patients = await response.Content.ReadFromJsonAsync<List<Patient>>() ?? new();
            }
        }
    }

}
