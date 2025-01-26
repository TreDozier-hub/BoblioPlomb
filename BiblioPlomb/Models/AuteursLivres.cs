using BiblioPlomb.Models;
namespace BiblioPlomb.Models
{
    public class AuteurLivre
    {        
        public int AuteurId { get; set; }
        public Auteur Auteur { get; set; } = default!;

        public int LivreId { get; set; }
        public Livre Livre { get; set; } = default!;
    }
}