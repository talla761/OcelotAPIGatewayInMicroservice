namespace RiskAssessment.Models;

public class AssessmentResult
{
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string RiskLevel { get; set; } = "None";
    public int Triggers { get; set; }
    public IEnumerable<string> TriggersFound { get; set; } = Array.Empty<string>();
}
