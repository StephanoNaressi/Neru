using Microsoft.EntityFrameworkCore;
using Neru.Models;
namespace Neru.Context
{
    public class SqliteContext : DbContext
    {
        public DbSet<UserRemembrance> UserRemembrances { get; set; }
        public DbSet<MyWiki> MyWikis { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlite("Data Source=Sqlite.db");
    }
}
