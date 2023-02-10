using System;
using System.ComponentModel.DataAnnotations;

namespace Cineflix.Models
{
	public class PurchasedMovie
	{
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;
        [DataType(DataType.Date)]
        public DateTime PurchasedAt { get; set; } = DateTime.Now;
        public Boolean HasWatched { get; set; }
    }
}

