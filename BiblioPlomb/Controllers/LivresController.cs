using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BiblioPlomb.Data;
using BiblioPlomb.Models;
using BiblioPlomb.DTO;

namespace BiblioPlomb.Controllers
{
    public class LivresController : Controller
    {
        private readonly BiblioPlombDB _context;

        public LivresController(BiblioPlombDB context)
        {
            _context = context;
        }

        // GET: Livres
        public async Task<IActionResult> Index()
        {
            var biblioPlombDB = _context.Livres.Include(livre => livre.Genre);
            return View(await biblioPlombDB.ToListAsync());
        }

        // GET: Livres/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var livre = await _context.Livres
                .Include(livre => livre.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (livre == null)
            {
                return NotFound();
            }

            return View(livre);
        }

        // GET: Livres/Create
        public IActionResult Create(string titre, int? genreId, int? auteurId)
        {
            // Définissez le titre pour la vue
            ViewData["Title"] = "Create Livre";

            // Préparez les données pour la vue
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Nom", genreId);
            ViewBag.Auteurs = _context.Auteurs.ToList();

            // Pré-remplir les champs du modèle si des données sont passées
            var livre = new Livre
            {
                Titre = titre,
                GenreId = genreId ?? 0,
                AuteurId = auteurId ?? 0
            };

            if (auteurId.HasValue)
            {
                var auteur = _context.Auteurs.Find(auteurId.Value);
                if (auteur != null)
                {
                    livre.Auteur = auteur.Nom;
                }
            }

            return View(livre);
        }


        //[HttpPost]
        //public async Task<IActionResult> Create(Livre livre)
        //{
        //    var auteur = await _context.Auteurs.FirstOrDefaultAsync(auteur => auteur.Nom == livre.Auteur);
        //    if (auteur == null)
        //    {
        //        // Redirect to author creation with a message and return URL
        //        string returnUrl = Url.Action("Create", "Livres", new { livre.Titre, livre.GenreId });
        //        string message = "L'auteur n'existe pas dans la base de données. Vous devez créer un nouvel auteur.";
        //        return RedirectToAction("Create", "Auteurs", new { nomAuteur = livre.Auteur, returnUrl, message });
        //    }

        //    livre.AuteurId = auteur.Id;
        //    _context.Livres.Add(livre);
        //    await _context.SaveChangesAsync();

        //    // Ajouter l'association dans AuteurLivre
        //    var auteurLivre = new AuteurLivre
        //    {
        //        AuteurId = livre.AuteurId,
        //        LivreId = livre.Id
        //    };
        //    _context.AuteurLivres.Add(auteurLivre);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction(nameof(Index));
        //}

        [HttpPost]
        public async Task<IActionResult> Create(Livre livre)
        {
            // Vérifie si l'auteur existe
            var auteur = await _context.Auteurs.FirstOrDefaultAsync(a => a.Nom == livre.Auteur);
            if (auteur == null)
            {
                // Générer l'URL de retour
                string returnUrl = Url.Action("Create", "Livres", new { livre.Titre, livre.GenreId });
                string message = "L'auteur n'existe pas dans la base de données. Veuillez créer cet auteur avant de continuer.";

                // Rediriger vers la création d'auteur
                return RedirectToAction("Create", "Auteurs", new { nomAuteur = livre.Auteur, returnUrl, message });
            }

            // Associer l'auteur existant au livre
            livre.AuteurId = auteur.Id;
            _context.Livres.Add(livre);
            await _context.SaveChangesAsync();

            // Ajouter l'association dans AuteurLivre
            var auteurLivre = new AuteurLivre
            {
                AuteurId = livre.AuteurId,
                LivreId = livre.Id
            };
            _context.AuteurLivres.Add(auteurLivre);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        // GET: Livres/Edit
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var livre = _context.Livres.Find(id);
            if (livre == null)
            {
                return NotFound();
            }

            ViewData["Title"] = "Edit Livre"; // Ajout du titre pour la vue
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Nom", livre.GenreId);
            ViewBag.Auteurs = _context.Auteurs.ToList();

            return View(livre);
        }


        // POST: Livres/Edit/
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Livre livre)
        {
            ModelState.Clear();

            if (!LivreExists(id))
            {
                return NotFound();
            }

            _context.Update(livre);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Livres/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var livre = await _context.Livres
                .Include(livre => livre.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (livre == null)
            {
                return NotFound();
            }

            return View(livre);
        }

        // POST: Livres/Delete
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var livre = await _context.Livres.FindAsync(id);
            if (livre != null)
            {
                _context.Livres.Remove(livre);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LivreExists(int id)
        {
            return _context.Livres.Any(e => e.Id == id);
        }

        [HttpGet]
        public async Task<IActionResult> ListeLivresAvecAuteurs()
        {
            var livres = await _context.Livres
                .Include(l => l.AuteurLivres)
                    .ThenInclude(al => al.Auteur)
                .Include(l => l.Genre)
                .ToListAsync();

            var livresDto = livres.Select(l => new AuteursLivresDTO
            {
                LivreTitre = l.Titre,
                AuteurNom = string.Join(", ", l.AuteurLivres.Select(al => al.Auteur.Nom))
            }).ToList();

            return View(livresDto);
        }

        [HttpPost]
        public async Task<IActionResult> ListeLivresAvecAuteurs(Livre livre)
        {
            var auteur = await _context.Auteurs.FirstOrDefaultAsync(a => a.Nom == livre.Auteur);
            if (auteur == null)
            {
                auteur = new Auteur { Nom = livre.Auteur };
                _context.Auteurs.Add(auteur);
                await _context.SaveChangesAsync();
            }
            livre.AuteurId = auteur.Id;

            _context.Add(livre);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListeLivresAvecAuteurs));
        }

        // GET: Livres/Rechercher
        [HttpGet]
        public async Task<IActionResult> Rechercher(string searchQuery)
        {
            var livres = await _context.Livres
                .Include(livre => livre.AuteurLivres)
                    .ThenInclude(al => al.Auteur)
                .Include(livre => livre.Genre)
                .Where(livre => livre.Titre.Contains(searchQuery) ||
                            livre.AuteurLivres.Any(al => al.Auteur.Nom.Contains(searchQuery)))
                .ToListAsync();

            var livresDto = livres.Select(livre => new AuteursLivresDTO
            {
                LivreTitre = livre.Titre,
                AuteurNom = string.Join(", ", livre.AuteurLivres.Select(al => al.Auteur.Nom))
            }).ToList();

            return PartialView("RechercheLi_Au", livresDto);
        }


        [HttpGet]
        public async Task<IActionResult> CherchAuteurs(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return Json(new List<object>());
            }

            var auteurs = await _context.Auteurs
                .Where(a => EF.Functions.Like(a.Nom, $"%{searchTerm}%"))
                .Select(a => new
                {
                    label = a.Nom,
                    value = a.Id
                })
                .ToListAsync();

            return Json(auteurs);
        }

    }
}