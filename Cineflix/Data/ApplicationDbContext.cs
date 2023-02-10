using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Cineflix.Models;

namespace Cineflix.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Cineflix.Models.Movie> Movie { get; set; } = default!;
    public DbSet<Cineflix.Models.PurchasedMovie> PurchasedMovie { get; set; } = default!;
}

