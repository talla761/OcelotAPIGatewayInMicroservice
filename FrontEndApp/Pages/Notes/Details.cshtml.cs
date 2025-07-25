using FrontEndApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FrontEndApp.Pages.Notes
{
    public class Details : PageModel
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly IConfiguration _config;

        public Details(IHttpClientFactory httpFactory, IConfiguration config)
        {
            _httpFactory = httpFactory;
            _config = config;
        }

        public Note? Note { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToPage("/Account/Login");

            var client = _httpFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var baseUrl = _config["Services:GatewayBaseUrl"];
            var response = await client.GetAsync($"{baseUrl}/api/notes/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            Note = await response.Content.ReadFromJsonAsync<Note>();
            return Page();
        }
    }
}
