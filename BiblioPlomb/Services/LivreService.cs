using BiblioPlomb.Data;
using BiblioPlomb.DTO;
using BiblioPlomb.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace BiblioPlomb.Services
{
    public class LivreService
    {
        private readonly BiblioPlombDB _db;

        public LivreService(BiblioPlombDB db)
        {
            _db = db;
        }

        // Crée un livre
        public async Task<IResult> AddLivre(LivreDTO livreDTO)
        {
            if (await _db.Genres.FindAsync(livreDTO.GenreId) == null ||
                !await _db.Auteurs.AnyAsync(a => a.Id == livreDTO.AuteurId))
            {
                return TypedResults.BadRequest("Genre ou Auteur non trouvé.");
            }

            var auteur = await _db.Auteurs.FindAsync(livreDTO.AuteurId);

            var livre = new Livre
            {
                Titre = livreDTO.Titre,
                Auteur = auteur?.Nom ?? string.Empty, // Mise à jour de l'auteur principal
                Dispo = livreDTO.Dispo,
                Etat = livreDTO.Etat,
                GenreId = livreDTO.GenreId,
                ISBN = livreDTO.ISBN
            };

            var auteurLivre = new AuteurLivre
            {
                Livre = livre,
                AuteurId = livreDTO.AuteurId
            };

            livre.AuteurLivres.Add(auteurLivre);

            _db.Livres.Add(livre);
            await _db.SaveChangesAsync();

            return TypedResults.Created($"/livres/{livre.Id}", livre);
        }

        // Récupérer un livre par Id avec ses relations
        public async Task<IResult> GetLivre(int id)
        {
            var livre = await _db.Livres
                .Include(livre => livre.Genre)
                .Include(livre => livre.AuteurLivres).ThenInclude(auteurlivre => auteurlivre.Auteur)
                .FirstOrDefaultAsync(livre => livre.Id == id);

            return livre == null ? TypedResults.NotFound() : TypedResults.Ok(livre);
        }

        // Liste des livres avec genres et auteurs
        public async Task<IResult> GetAllLivres()
        {
            var livres = await _db.Livres
                .Include(livre => livre.Genre)
                .Include(livre => livre.AuteurLivres).ThenInclude(auteurlivre => auteurlivre.Auteur)
                .ToListAsync();

            return TypedResults.Ok(livres);
        }

        // Vérifie si un livre est disponible
        public async Task<IResult> VerifDispo(string titre)
        {
            var livres = await _db.Livres
                .Where(livre => livre.Titre == titre)
                .ToListAsync();

            return livres.Any(livre => livre.Dispo)
                ? TypedResults.Ok("Le livre est là, héhé.")
                : TypedResults.Ok("Oups... Le livre est indisponible.");
        }

        //Supprimer un livre(endommagé par exemple)
        public async Task<IResult> DeleteLivre(int id)
        {
            var livre = await _db.Livres.FindAsync(id);
            if (livre == null)
            {
                return TypedResults.NotFound();
            }

            _db.Livres.Remove(livre);
            await _db.SaveChangesAsync();
            return TypedResults.NoContent();
        }

        // Modifier l'état du livre
        public async Task<IResult> ModifierEtatLivre(int id, EtatLivre nouvelEtat)
        {
            var livre = await _db.Livres.FindAsync(id);
            if (livre == null)
            {
                return TypedResults.NotFound();
            }

            // Logique pour éviter l'amélioration de l'état du livre une fois dégradé
            if ((livre.Etat == EtatLivre.Dégradé) ||
                (livre.Etat == EtatLivre.Moyen && nouvelEtat == EtatLivre.Bon))
            {
                return TypedResults.BadRequest("Le livre ne peut pas changé état meilleur.");
            }

            livre.Etat = nouvelEtat;

            if (nouvelEtat == EtatLivre.Dégradé)
            {
                livre.Dispo = false; // Inempruntable
            }
            else
            {
                livre.Dispo = true; // Empruntable
            }

            await _db.SaveChangesAsync();
            return TypedResults.Ok(livre);
        }

        //public async Task ImporJsonLivres(string filePath)
        //{
        //    if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
        //    {
        //        throw new FileNotFoundException($"Le fichier spécifié est introuvable : {filePath}");
        //    }

        //    try
        //    {
        //        // Lire le contenu du fichier JSON
        //        var jsonData = await File.ReadAllTextAsync(filePath);

        //        // Désérialiser les données JSON en une liste d'objets
        //        var livres = JsonSerializer.Deserialize<List<Livre>>(jsonData, new JsonSerializerOptions
        //        {
        //            PropertyNameCaseInsensitive = true // Pour ignorer la casse dans les noms de propriétés JSON
        //        });

        //        if (livres != null && livres.Any())
        //        {
        //            // Ajouter les livres dans la base de données
        //            _db.Livres.AddRange(livres);

        //            // Sauvegarder les changements
        //            await _db.SaveChangesAsync();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Gérer les erreurs
        //        Console.WriteLine($"Erreur lors de l'importation du fichier JSON : {ex.Message}");
        //        throw;
        //    }
        //}

    }
}
