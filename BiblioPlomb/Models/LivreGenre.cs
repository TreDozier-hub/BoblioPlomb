using BiblioPlomb.Models;

namespace BiblioPlomb.Models
{
    public class LivreGenre
    {
        public int LivreId { get; set; }
        public Livre Livre { get; set; }

        public int GenreId { get; set; }
        public Genre Genre { get; set; }
    }
}
