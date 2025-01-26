using BiblioPlomb.Models;

namespace BiblioPlomb.Models
{
    public class Auteur
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;

        // Ajoute les livres de cet auteur
        public virtual ICollection<AuteurLivre> AuteurLivres { get; set; } = new List<AuteurLivre>();
    }
}
