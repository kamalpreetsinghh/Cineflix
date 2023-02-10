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
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (_context.Movie == null) {
                return Problem("Entity set 'ApplicationDbContext.Movie'  is null.");
            } else {
                var movies = await _context.Movie.ToListAsync();
                var viewModelMovies = new List<MovieViewModel>();
                movies.ForEach(movie => viewModelMovies.Add(new MovieViewModel(movie)));
                var userEmail = User.Identity.Name;
                if (_context.PurchasedMovie != null && userEmail != null)
                {
                    var allPurchasedMovies = await _context.PurchasedMovie.ToListAsync();
                    var userMovies = allPurchasedMovies.Where(movie => movie.UserId == userEmail);
                    if(userMovies != null)
                    {
                        foreach(var userMovie in userMovies)
                        {
                            foreach(var viewModelMovie in viewModelMovies)
                            {
                                if(viewModelMovie.Movie.Id == userMovie.MovieId)
                                {
                                    viewModelMovie.IsPurchased = true;
                                }
                            }
                        }
                    }
                }
                
                return View(viewModelMovies);
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }
            var movieViewModel = new MovieViewModel(movie);
            var userEmail = User.Identity.Name;
            if (_context.PurchasedMovie != null && userEmail != null)
            {
                var userMovie = await _context.PurchasedMovie
                    .FirstOrDefaultAsync(m => m.MovieId == movie.Id && m.UserId == userEmail);
                if(userMovie != null)
                {
                    movieViewModel.IsPurchased = true;
                }
            }

            return View(movieViewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ReleaseDate,Genre,Price,FileUpload")] MovieCreateViewModel movieCreateViewModel)
        {
            if (ModelState.IsValid)
            {
                Movie movie = new Movie();
                movie.Title = movieCreateViewModel.Title;
                movie.ReleaseDate = movieCreateViewModel.ReleaseDate;
                movie.Genre = movieCreateViewModel.Genre;
                movie.Price = movieCreateViewModel.Price;
                    
                using (var ms = new MemoryStream())
                {
                    if (movieCreateViewModel.FileUpload != null)
                    {
                        movieCreateViewModel.FileUpload.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        string byte64 = Convert.ToBase64String(fileBytes);
                        movie.MovieThumbnail = byte64;
                    }
                }

                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movieCreateViewModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ReleaseDate,Genre,Price,MovieThumbnail")] Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
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
            return View(movie);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Movie == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Movie'  is null.");
            }
            var movie = await _context.Movie.FindAsync(id);
            if (movie != null)
            {
                _context.Movie.Remove(movie);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Play()
        {
            return View();
        }

        private bool MovieExists(int id)
        {
          return (_context.Movie?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
