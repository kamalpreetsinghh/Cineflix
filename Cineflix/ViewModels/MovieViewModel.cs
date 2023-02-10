using System;
using Cineflix.Models;
namespace Cineflix.ViewModels;

public class MovieViewModel
{
	public Movie Movie { get; set; } = null!;
	public bool IsPurchased { get; set; }

	public MovieViewModel(Movie movie)
	{
		Movie = movie;
	}
}

