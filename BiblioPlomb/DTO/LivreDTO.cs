using BiblioPlomb.Models;
using System.ComponentModel.DataAnnotations;

namespace BiblioPlomb.DTO
{
    public class LivreDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Le titre ne peut pas dépasser 50 caractères.")]
        public string Titre { get; set; } = string.Empty;

        public bool Dispo { get; set; }
        public EtatLivre Etat { get; set; }

        [Required]
        [Range(1000000000000, 9999999999999, ErrorMessage = "ISBN doit avoir 13 chiffres.")]
        public long ISBN { get; set; }

        [Required]
        public int GenreId { get; set; }

        [Required]
        public int AuteurId { get; set; }
    }
}