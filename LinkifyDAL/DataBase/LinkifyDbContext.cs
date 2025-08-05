using LinkifyDAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace LinkifyDAL.DataBase
{
    public class LinkifyDbContext: IdentityDbContext
    {
        public LinkifyDbContext(DbContextOptions<LinkifyDbContext> options) : base(options)
        { }
        public DbSet<User> User { get; set; }
        public DbSet<Friends> Friends { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<PostReactions> PostReactions { get; set; }
        public DbSet<PostImages> PostImages { get; set; }
        public DbSet<PostComments> PostComments { get; set; }
        public DbSet<Contact> Contacts { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PostComments>()
        .HasOne(pc => pc.Post)
        .WithMany() 
        .HasForeignKey(pc => pc.PostId)
        .OnDelete(DeleteBehavior.Restrict);

            // 🟢 PostComments fix
            modelBuilder.Entity<PostComments>()
                .HasOne(pc => pc.User)
                .WithMany() 
                .HasForeignKey(pc => pc.CommenterId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<PostComments>()
                .HasOne(pc => pc.ParentComment)
                .WithMany() // assuming no navigation from Parent → Children
                .HasForeignKey(pc => pc.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);



            // 🟢 Friends fix
            modelBuilder.Entity<Friends>()
                .HasOne(f => f.Requester)
                .WithMany()
                .HasForeignKey(f => f.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friends>()
                .HasOne(f => f.Addressee)
                .WithMany()
                .HasForeignKey(f => f.AddresseeId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<PostReactions>()
    .HasOne(r => r.Post)
    .WithMany() 
    .HasForeignKey(r => r.PostId)
    .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<PostReactions>()
                .HasOne(r => r.Reactor)
                .WithMany() 
                .HasForeignKey(r => r.ReactorId)
                .OnDelete(DeleteBehavior.Restrict); 
        }

    
    }
}
