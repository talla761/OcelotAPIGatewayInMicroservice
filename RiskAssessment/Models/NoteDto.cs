namespace RiskAssessment.Models;

public class NoteDto
{
    public string Id { get; set; } = string.Empty;
    public int PatId { get; set; }
    public string Content { get; set; } = string.Empty;
}
