using System.Data.Entity;
using MovieBrowser.Domain.Entities;

namespace MovieBrowser.Domain.Concrete
{
    public class EFDbContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }
    }
}
