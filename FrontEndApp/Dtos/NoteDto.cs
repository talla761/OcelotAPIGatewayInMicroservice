namespace FrontEndApp.Dtos
{
    public class NoteDto
    {
        public string? Id { get; set; }
        public int PatientId { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
