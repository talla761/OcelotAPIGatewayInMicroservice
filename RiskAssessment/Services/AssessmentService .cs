using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using RiskAssessment.Models;

namespace RiskAssessment.Services
{
    public class AssessmentService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        private readonly string[] _triggers = new[]
        {
            "Hémoglobine A1C", "Microalbumine", "Taille", "Poids",
            "Fumeur", "Fumeuse", "Anormal", "Cholestérol",
            "Vertiges", "Rechute", "Réaction", "Anticorps"
        };

        public AssessmentService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> EvaluateAsync(int patientId, string token)
        {
            try
            {
                // Configuration via ocelot.json ou appsettings.json
                var patientServiceUrl = _configuration["Downstreams:Patient"];
                var notesServiceUrl = _configuration["Downstreams:Notes"];

                // Prépare les headers
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Appel au microservice Patient
                var patient = await _httpClient.GetFromJsonAsync<PatientDto>($"{patientServiceUrl}/api/patient/{patientId}");
                if (patient == null) throw new Exception("Patient introuvable");

                // Appel au microservice Notes
                var notes = await _httpClient.GetFromJsonAsync<List<NoteDto>>($"{notesServiceUrl}/api/notes/patient/{patientId}");
                if (notes == null) throw new Exception("Aucune note trouvée");

                // Compte les déclencheurs dans les notes (en ignorant la casse)
                var triggerCount = notes
                    .Select(n => n.Content.ToLowerInvariant())
                    .Sum(note => _triggers.Count(trigger =>
                        note.Contains(trigger.ToLowerInvariant())));

                var age = DateTime.Now.Year - patient.DateNaissance.Year;
                if (patient.DateNaissance > DateTime.Now.AddYears(-age)) age--;

                // Application des règles
                string riskLevel = "None";
                if (triggerCount == 0)
                {
                    riskLevel = "None";
                }
                else if (triggerCount >= 2 && triggerCount <= 5 && age > 30)
                {
                    riskLevel = "Borderline";
                }
                else if (
                    (patient.Genre.ToLower() == "m" && age < 30 && triggerCount == 3) ||
                    (patient.Genre.ToLower() == "f" && age < 30 && triggerCount == 4) ||
                    (age >= 30 && triggerCount == 6 && triggerCount == 7)
                )
                {
                    riskLevel = "In Danger";
                }
                else if (
                    (patient.Genre.ToLower() == "m" && age < 30 && triggerCount >= 5) ||
                    (patient.Genre.ToLower() == "f" && age < 30 && triggerCount >= 7) ||
                    (age >= 30 && triggerCount >= 8)
                )
                {
                    riskLevel = "Early onset";
                }

                return riskLevel;
            }
            catch (Exception ex)
            {
                // Journaliser l'erreur si nécessaire
                throw new Exception($"Échec de l'évaluation du risque : {ex.Message}");
            }
        }
    }
}
