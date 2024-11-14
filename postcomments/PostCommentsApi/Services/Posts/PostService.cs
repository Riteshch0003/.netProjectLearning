using Microsoft.EntityFrameworkCore;
using PostCommentsApi.Data;
using PostCommentsApi.Models;

namespace PostCommentsApi.Services
{
    public class PostService : IPostService
    {
        private readonly PostCommentsContext _context;

        public PostService(PostCommentsContext context)
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

        
    }
}
