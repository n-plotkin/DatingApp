using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options) {

        }
        //in a DbContext, a DbSet represents a table in the relational database.
        public DbSet<AppUser> Users { get; set; }
    }
}