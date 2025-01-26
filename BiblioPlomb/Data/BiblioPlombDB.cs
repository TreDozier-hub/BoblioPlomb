using Microsoft.EntityFrameworkCore;
using BiblioPlomb.Models;
//using BiblioPlomb.Models;


namespace BiblioPlomb.Data
{
    public class BiblioPlombDB : DbContext
    {
        public BiblioPlombDB(DbContextOptions<BiblioPlombDB> options)
            : base(options) { }

        public DbSet<Livre> Livres { get; set; }
        public DbSet<Auteur> Auteurs { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<AuteurLivre> AuteurLivres { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AuteurLivre>()
                .HasKey(auteurlivre => new { auteurlivre.AuteurId, auteurlivre.LivreId });

            modelBuilder.Entity<AuteurLivre>()
                .HasOne(auteurlivre => auteurlivre.Auteur)
                .WithMany(a => a.AuteurLivres)
                .HasForeignKey(auteurlivre => auteurlivre.AuteurId);

            modelBuilder.Entity<AuteurLivre>()
                .HasOne(auteurlivre => auteurlivre.Livre)
                .WithMany(livre => livre.AuteurLivres)
                .HasForeignKey(auteurlivre => auteurlivre.LivreId);



            modelBuilder.Entity<Livre>()
                .HasOne(livre => livre.Genre)
                .WithMany(genre => genre.Livres)
                .HasForeignKey(livre => livre.GenreId);

            //modelBuilder.Entity<Genre>()
            //    .HasOne(genre => genre.Livres)
            //    .WithMany(livre => livre.Genres)
            //    .HasForeignKey(genre => genre.LivreId);


        }
    }
}
