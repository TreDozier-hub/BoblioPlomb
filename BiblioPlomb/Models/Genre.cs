namespace BiblioPlomb.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public ICollection<Livre> Livres { get; set; } = new List<Livre>();
    }
}
