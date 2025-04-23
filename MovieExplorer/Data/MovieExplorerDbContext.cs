using Microsoft.EntityFrameworkCore;
using MovieExplorer.Models;

namespace MovieExplorer.Data
{
    public class MovieExplorerDbContext(DbContextOptions<MovieExplorerDbContext> options) : DbContext(options)
    {
        public DbSet<Comment> Comments { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
