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
        public DbSet<MemberLike> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // the key for this table is going to be made up of the sourceMemberId and targetMemberId
            modelBuilder.Entity<MemberLike>().HasKey(x => new { x.SourceMemberId, x.TargetMemberId });
            modelBuilder.Entity<MemberLike>().HasOne(s => s.SourceMember)
                .WithMany(t => t.LikedMembers).HasForeignKey(s => s.SourceMemberId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MemberLike>().HasKey(x => new { x.SourceMemberId, x.TargetMemberId });
            modelBuilder.Entity<MemberLike>().HasOne(s => s.TargetMember)
                .WithMany(t => t.LikedByMembers).HasForeignKey(s => s.TargetMemberId)
                .OnDelete(DeleteBehavior.NoAction);
        }


    }
}
