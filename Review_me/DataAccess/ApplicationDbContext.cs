using Microsoft.EntityFrameworkCore;
using Review_me.Models;

namespace Review_me.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }

        public DbSet<BuyLink> BuyLinks { get; set; }

        public DbSet<ReviewLink> ReviewLinks { get; set; }

        public DbSet<Isbn> Isbns { get; set; }


    }
}