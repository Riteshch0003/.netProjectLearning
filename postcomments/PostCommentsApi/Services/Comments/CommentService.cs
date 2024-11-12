// Services/CommentService.cs
using Microsoft.EntityFrameworkCore;
using PostCommentsApi.Data;
using PostCommentsApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PostCommentsApi.Services
{
    public class CommentService : ICommentService
    {
        private readonly AppDbContext _context;

        public CommentService(AppDbContext context)
        {
            _context = context;
        }

        // Add a comment to a post
        public async Task<Comment> AddCommentAsync(int postId, Comment comment)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
            {
                throw new KeyNotFoundException("Post not found.");
            }

            comment.PostId = postId;
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        // Get all comments for a specific post
        public async Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(int postId)
        {
            return await _context.Comments
                                 .Where(c => c.PostId == postId)
                                 .ToListAsync();
        }
    }
}
