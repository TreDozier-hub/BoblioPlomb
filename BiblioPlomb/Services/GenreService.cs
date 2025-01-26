using BiblioPlomb.Data;
using BiblioPlomb.DTO;
using BiblioPlomb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
//using BiblioPlomb.DTO;
//using BiblioPlomb.Models;

namespace BiblioPlomb.Services
{
    public class GenreService
    {
        private readonly BiblioPlombDB _db;

        public GenreService(BiblioPlombDB db)
        {
            _db = db;
        }

        // Créer un genre
        public async Task<IResult> AddGenre(GenreDTO genreDTO)
        {
            //List<Genre> listeGenre = new List<Genre>();            
                
            var genre = new Genre
            {

                Nom = genreDTO.Nom
            };

            _db.Genres.Add(genre);
            await _db.SaveChangesAsync();

            return TypedResults.Created($"/genres/{genre.Id}", genre);
        }

        // genre par ID
        public async Task<IResult> GetGenre(int id)
        {
            var genre = await _db.Genres
                .FirstOrDefaultAsync(genre => genre.Id == id);

            return genre == null ? TypedResults.NotFound() : TypedResults.Ok(genre);
        }

        // Liste tous les genres
        public async Task<IResult> GetAllGenres()
        {
            var genres = await _db.Genres.ToListAsync();
            return TypedResults.Ok(genres);
        }

        // Modifier un genre
        public async Task<IResult> UpdateGenre(int id, GenreDTO genreDTO)
        {
            var genre = await _db.Genres.FindAsync(id);
            if (genre == null)
            {
                return TypedResults.NotFound();
            }

            genre.Nom = genreDTO.Nom;

            await _db.SaveChangesAsync();
            return TypedResults.NoContent();
        }

        // Supprimer un genre
        public async Task<IResult> DeleteGenre(int id)
        {
            var genre = await _db.Genres.FindAsync(id);
            if (genre == null)
            {
                return TypedResults.NotFound();
            }

            _db.Genres.Remove(genre);
            await _db.SaveChangesAsync();
            return TypedResults.NoContent();
        }
    }
}