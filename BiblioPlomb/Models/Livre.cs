using BiblioPlomb.Models;

namespace BiblioPlomb.Models
{
    public enum EtatLivre
    {
        Bon,
        Moyen,
        Dégradé
    }

    public class Livre
    {
        public int Id { get; set; }
        public string Titre { get; set; } = string.Empty;
        public string Auteur { get; set; } = string.Empty;
        public bool Dispo { get; set; }
        public EtatLivre Etat { get; set; }
        public long ISBN { get; set; }
        public int GenreId { get; set; }
        public int AuteurId { get; set; }
        public Genre Genre { get; set; }
        public ICollection<AuteurLivre> AuteurLivres { get; set; } = new List<AuteurLivre>();

        public void ModifierEtat(EtatLivre nouvelEtat)
        {
            // Logique pour s'assurer qu'un livre ne peut pas revenir à un état meilleur
            if (Etat == EtatLivre.Dégradé)
                return;

            if (Etat == EtatLivre.Moyen && nouvelEtat == EtatLivre.Bon)
                return;

            Etat = nouvelEtat;
        }
    }

}

