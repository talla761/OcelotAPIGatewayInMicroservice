using System.ComponentModel.DataAnnotations;

namespace Patient.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Prenom { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nom { get; set; }

        [Required]
        public DateTime DateNaissance { get; set; }

        [Required]
        public string Genre { get; set; } // Exemple : "Homme", "Femme", "Autre"

        // Les deux champs suivants sont optionnels

        [MaxLength(250)]
        public string AdressePostale { get; set; }

        [Phone]
        [MaxLength(20)]
        public string NumeroTelephone { get; set; }
    }
}
