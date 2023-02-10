using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cineflix.Data;
using Cineflix.Models;
using Cineflix.ViewModels;

namespace Cineflix.Controllers
{
    public class PurchaseMoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchaseMoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userEmail = User.Identity.Name;
            var allPurchasedMovies = await _context.PurchasedMovie.Include(m => m.Movie).ToListAsync();
            var userMovies = allPurchasedMovies.Where(movie => movie.UserId == userEmail);
            var viewModelMovies = new List<MovieViewModel>();
            foreach (var movie in userMovies) {
                viewModelMovies.Add(new MovieViewModel(movie.Movie));
            }
            return View(viewModelMovies);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.PurchasedMovie == null)
            {
                return NotFound();
            }

            var purchasedMovie = await _context.PurchasedMovie
                .Include(p => p.Movie)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchasedMovie == null)
            {
                return NotFound();
            }

            return View(purchasedMovie);
        }

        public async Task<IActionResult> Purchase(int id)
        {
            PurchasedMovie movie = new PurchasedMovie();
            var userEmail = User.Identity.Name;
            if (ModelState.IsValid && userEmail != null)
            {
                movie.MovieId = (int)id;
                movie.UserId = (string)userEmail;
                _context.Add(movie);
                var task = await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Movies");
            }
            return RedirectToAction("Index", "Movies");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PurchasedMovie == null)
            {
                return NotFound();
            }

            var purchasedMovie = await _context.PurchasedMovie.FindAsync(id);
            if (purchasedMovie == null)
            {
                return NotFound();
            }
            ViewData["MovieId"] = new SelectList(_context.Movie, "Id", "Id", purchasedMovie.MovieId);
            return View(purchasedMovie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,MovieId,PurchasedAt,HasWatched")] PurchasedMovie purchasedMovie)
        {
            if (id != purchasedMovie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(purchasedMovie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchasedMovieExists(purchasedMovie.Id))
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
            ViewData["MovieId"] = new SelectList(_context.Movie, "Id", "Id", purchasedMovie.MovieId);
            return View(purchasedMovie);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PurchasedMovie == null)
            {
                return NotFound();
            }

            var purchasedMovie = await _context.PurchasedMovie
                .Include(p => p.Movie)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchasedMovie == null)
            {
                return NotFound();
            }

            return View(purchasedMovie);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PurchasedMovie == null)
            {
                return Problem("Entity set 'ApplicationDbContext.PurchasedMovie'  is null.");
            }
            var purchasedMovie = await _context.PurchasedMovie.FindAsync(id);
            if (purchasedMovie != null)
            {
                _context.PurchasedMovie.Remove(purchasedMovie);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PurchasedMovieExists(int id)
        {
          return (_context.PurchasedMovie?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
