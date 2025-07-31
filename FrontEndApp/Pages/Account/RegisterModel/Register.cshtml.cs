using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace FrontEndApp.Pages.Account.RegisterModel
{
    public class Register : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "Le nom d'utilisateur est requis.")]
        public string Username { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Le mot de passe est requis.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Veuillez confirmer le mot de passe.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas.")]
        public string ConfirmPassword { get; set; }

        public string? ErrorMessage { get; set; }

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public Register(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var registerData = new
            {
                Email = Username, // ou Username selon ton modèle backend
                Password,
                ConfirmPassword
            };

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync(
                _configuration["Services:GatewayBaseUrl"] + "/api/account/register",
                registerData
            );

            if (response.IsSuccessStatusCode)
            {
                // Succès ? Redirection vers la page de connexion
                return RedirectToPage("/Account/LoginModel/Login");
            }
            else
            {
                // Lecture du message d'erreur depuis l'API
                var errorJson = await response.Content.ReadAsStringAsync();
                try
                {
                    var doc = JsonDocument.Parse(errorJson);
                    ErrorMessage = doc.RootElement.GetProperty("message").GetString();
                }
                catch
                {
                    ErrorMessage = "Échec de l'inscription.";
                }

                return Page();
            }

        }
    }

}
