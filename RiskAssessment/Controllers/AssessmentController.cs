using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RiskAssessment.Models;
using RiskAssessment.Services;

namespace RiskAssessment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]                                // ??  Protégé par Bearer JWT
    public class AssessmentController : ControllerBase
    {
        private readonly AssessmentService _svc;

        public AssessmentController(AssessmentService svc)
        {
            _svc = svc;
        }

        /// <summary>
        /// Calcule et renvoie le niveau de risque de diabète d’un patient
        /// </summary>
        /// <param name="patientId">Identifiant du patient</param>
        /// <returns>JSON : { patientId, riskLevel, triggerCount }</returns>
        [HttpGet("{patientId:int}")]
        public async Task<IActionResult> Get(int patientId)
        {
            // Récupère le token de l’en?tête Authorization : Bearer xxx
            var authHeader = Request.Headers.Authorization.ToString();
            if (!authHeader.StartsWith("Bearer "))
                return Unauthorized("Token manquant");

            var token = authHeader.Replace("Bearer ", "");

            // Appel du service
            var riskLevel = await _svc.EvaluateAsync(patientId, token);

            return Ok(new
            {
                patientId,
                riskLevel
            });
        }
    }
}
