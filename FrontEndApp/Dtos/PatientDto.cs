namespace FrontEndApp.Dtos
{
    public class PatientDto
    {
        public int PatientId { get; set; }
        public string Prenom { get; set; }
        public string Nom { get; set; }
        public DateTime DateNaissance { get; set; }
        public string Genre { get; set; }
        public string AdressePostale { get; set; }
        public string NumeroTelephone { get; set; }
    }
}
