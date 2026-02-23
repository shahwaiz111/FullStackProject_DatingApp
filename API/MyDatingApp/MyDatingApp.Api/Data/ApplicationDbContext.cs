using Microsoft.EntityFrameworkCore;
using MyDatingApp.Api.Entities;

namespace MyDatingApp.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Photo> Photos { get; set; }


    }
}
