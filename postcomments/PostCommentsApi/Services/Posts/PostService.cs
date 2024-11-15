using Microsoft.EntityFrameworkCore;
using PostCommentsApi.Data;
using PostCommentsApi.Models;
using MySql.Data.MySqlClient;


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
        public async Task<IEnumerable<Post>> GetPostsByUserIdAsync(int userId)
        {
            // Fetch posts by the userId
            var posts = _context.Posts.Where(p => p.UserId == userId).ToList();
            return await Task.FromResult(posts);
        }

        public async Task<Post> AddPost(int userId, Post post)
        {
           _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return post; 
        }

        public async Task<Post> CreatePostAsync(Post post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        
    }
}
