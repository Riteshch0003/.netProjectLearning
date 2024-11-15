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
    // Validate input
    if (post == null || string.IsNullOrWhiteSpace(post.Title) || string.IsNullOrWhiteSpace(post.Content))
    {
        throw new ArgumentException("Post title and content are required.");
    }

    // Associate the post with the userId
    post.UserId = userId;

    try
    {
        // Add the post to the database
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        // Return the created post
        return post;
    }
    catch (Exception ex)
    {
        // Log the exception (optional, depending on your logging strategy)
        // Handle or rethrow the exception
        throw new InvalidOperationException("An error occurred while adding the post.", ex);
    }
}


        public async Task<Post> CreatePostAsync(Post post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<Post> UpdatePostAsync(int userId, int postId, UpdatePostDto updatedPostDto)
{
    if (updatedPostDto == null || string.IsNullOrWhiteSpace(updatedPostDto.Title) || string.IsNullOrWhiteSpace(updatedPostDto.Content))
    {
        throw new ArgumentException("Post title and content are required.");
    }

    var existingPost = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId && p.UserId == userId);
    if (existingPost == null)
    {
        throw new KeyNotFoundException($"Post with ID {postId} for User ID {userId} not found.");
    }

    // Update post properties
    existingPost.Title = updatedPostDto.Title;
    existingPost.Content = updatedPostDto.Content;

    try
    {
        await _context.SaveChangesAsync(); // Save changes to the database
        return existingPost;
    }
    catch (Exception ex)
    {
        // Log the exception (use a logger)
        throw new Exception("An error occurred while updating the post.", ex);
    }
}
    }
}
