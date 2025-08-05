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

    }
}
