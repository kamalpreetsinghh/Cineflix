using System;
using System.ComponentModel.DataAnnotations;

namespace Cineflix.ViewModels
{
	public class MovieCreateViewModel
	{
        public string Title { get; set; } = null!;
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }
        public string Genre { get; set; } = null!;
        public decimal Price { get; set; }
        public IFormFile? FileUpload { get; set; }
    }
}