using BiblioPlomb.Data;
using BiblioPlomb.DTO;
using BiblioPlomb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace BiblioPlomb.Services
{
    public class AuteurService
    {
        private readonly BiblioPlombDB _db;

        public AuteurService(BiblioPlombDB db)
        {
            _db = db;
        }

        // nouvel auteur
        public async Task<IResult> AddAuteur(AuteurDTO auteurDTO)
        {
            var auteur = new Auteur
            {
                Nom = auteurDTO.Nom,
                Prenom = auteurDTO.Prenom
            };

            _db.Auteurs.Add(auteur);
            await _db.SaveChangesAsync();
            return TypedResults.Created($"/auteurs/{auteur.Id}", auteur);
        }

        // Récupérer un auteur par Id
        public async Task<IResult> GetAuteur(int id)
        {
            var auteur = await _db.Auteurs
                .Include(auteur => auteur.AuteurLivres)
                .FirstOrDefaultAsync(auteur => auteur.Id == id);

            return auteur == null ? TypedResults.NotFound() : TypedResults.Ok(auteur);
        }

        // Recherche par nom (insensible à la casse au cas ou)
        public async Task<IResult> AuteurParNom(string nom)
        {
            var auteur = await _db.Auteurs
                .Include(auteur => auteur.AuteurLivres)
                .FirstOrDefaultAsync(auteur => EF.Functions.Like(auteur.Nom, $"%{nom}%"));

            return auteur == null ? TypedResults.NotFound() : TypedResults.Ok(auteur);
        }

        //tous les auteurs
        public async Task<IResult> GetAllAuteurs()
        {
            var auteurs = await _db.Auteurs
                .Include(auteur => auteur.AuteurLivres)
                .ToListAsync();

            return TypedResults.Ok(auteurs);
        }

        // Modifier auteur
        public async Task<IResult> UpdateAuteur(int id, AuteurDTO auteurDTO)
        {
            var auteur = await _db.Auteurs.FindAsync(id);
            if (auteur == null)
            {
                return TypedResults.NotFound();
            }

            auteur.Nom = auteurDTO.Nom;
            //auteur.Prenom = auteurDTO.Prenom;

            await _db.SaveChangesAsync();
            return TypedResults.NoContent();
        }

        // Supprimer auteur
        public async Task<IResult> DeleteAuteur(int id)
        {
            var auteur = await _db.Auteurs.FindAsync(id);
            if (auteur == null)
            {
                return TypedResults.NotFound();
            }

            _db.Auteurs.Remove(auteur);
            await _db.SaveChangesAsync();
            return TypedResults.NoContent();
        }
    }
}