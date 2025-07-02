namespace RiskAssessment.Models;

public class PatientDto
{
    public int PatientId { get; set; }
    public string Prenom { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public DateTime DateNaissance { get; set; }
    public string Genre { get; set; } = string.Empty; // "Homme" ou "Femme"
}
