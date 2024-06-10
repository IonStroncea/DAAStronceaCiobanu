using AuthService.Entities;
using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace AuthService.DAL
{
    public class AuthContext : DbContext
    {
        public DbSet<Role> Roles { get; set; }

        public DbSet<Streamer> Streamers { get; set; }

        public AuthContext(DbContextOptions<AuthContext> options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Streamer>().HasKey(x => x.Id);
            modelBuilder.Entity<Role>().HasKey(x => x.Id);
        }
    }
}
