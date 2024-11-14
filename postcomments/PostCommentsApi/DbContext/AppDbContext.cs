using Microsoft.EntityFrameworkCore;
using PostCommentsApi.Models;

namespace PostCommentsApi.Data
{
    public class PostCommentsContext : DbContext
    {
        public PostCommentsContext(DbContextOptions<PostCommentsContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}
