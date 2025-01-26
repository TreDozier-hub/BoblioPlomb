using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BiblioPlomb.Data;
using BiblioPlomb.Models;
using static System.Net.Mime.MediaTypeNames;

namespace BiblioPlomb.Controllers
{
    public class AuteursController : Controller
    {
        private readonly BiblioPlombDB _context;

        public AuteursController(BiblioPlombDB context)
        {
            _context = context;
        }

        // GET: Auteur
        public async Task<IActionResult> Index()
        {
            return View(await _context.Auteurs.ToListAsync());
        }

        // GET: Auteurs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auteur = await _context.Auteurs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (auteur == null)
            {
                return NotFound();
            }

            return View(auteur);
        }

        // GET: Auteurs/Create
        public IActionResult Create(string nomAuteur, string returnUrl, string message)
        {
            // Initialisez les valeurs utilisées dans la vue
            ViewBag.ReturnUrl = returnUrl; // URL de retour
            ViewBag.Message = message;    // Message d'information
            ViewData["Title"] = "Create Auteur"; // Titre de la page

            // Pré-remplissez le modèle si nécessaire
            var auteur = new Auteur
            {
                Nom = nomAuteur
            };

            return View(auteur); // Passez le modèle à la vue
        }



        //// POST: Auteurs/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //// [Bind] restreindre les propriétés de modèle qui sont mises à jour lors d'une opération de liaison modèle
        //// comme lors d'une soumission de formulaire.
        //public async Task<IActionResult> Create([Bind("Id,Nom, Prenom")] Auteur auteur, string returnUrl = null)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(auteur);
        //        await _context.SaveChangesAsync();

        //        if (!string.IsNullOrEmpty(returnUrl))
        //        {
        //            return Redirect(returnUrl);
        //        }

        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(auteur);
        //}

        [HttpPost]
        public async Task<IActionResult> Create(Auteur auteur, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(auteur);
            }

            _context.Auteurs.Add(auteur);
            await _context.SaveChangesAsync();

            // Rediriger vers la création de livre avec l'auteur ajouté
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl + $"&AuteurId={auteur.Id}");
            }

            return RedirectToAction(nameof(Index));
        }



        // GET: Auteurs/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auteur = await _context.Auteurs.FindAsync(id);
            if (auteur == null)
            {
                return NotFound();
            }
            return View(auteur);
        }

        // POST: Auteurs/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nom ,Prenom")] Auteur auteur)
        {
            if (id != auteur.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(auteur);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuteurExists(auteur.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(auteur);
        }

        // GET: Auteurs/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auteur = await _context.Auteurs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (auteur == null)
            {
                return NotFound();
            }

            return View(auteur);
        }

        // POST: Auteurs/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var auteur = await _context.Auteurs.FindAsync(id);
            if (auteur != null)
            {
                _context.Auteurs.Remove(auteur);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuteurExists(int id)
        {
            return _context.Auteurs.Any(e => e.Id == id);
        }
    }
}