using Microsoft.EntityFrameworkCore;
using PostCommentsApi.Data;
using PostCommentsApi.Models;

namespace PostCommentsApi.Services
{
    public class PostService : IPostService
    {
        private readonly AppDbContext _context;

        public PostService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Post>> GetPostsAsync()
        {
            return await _context.Posts.Include(p => p.Comments).ToListAsync();
        }

        public async Task<Post> GetPostByIdAsync(int id)
        {
            return await _context.Posts.Include(p => p.Comments)
                                       .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Post> CreatePostAsync(Post post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<Comment> CreateCommentAsync(Comment comment)
        {
        // Ensure the PostId exists in the database
        var postExists = await _context.Posts.AnyAsync(p => p.Id == comment.PostId);
        if (!postExists)
        {
            throw new Exception("Post not found.");
        }

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        return comment;
    }
  
    }
}
