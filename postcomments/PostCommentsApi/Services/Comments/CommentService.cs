using Microsoft.EntityFrameworkCore;
using PostCommentsApi.Data;
using PostCommentsApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PostCommentsApi.Services
{
    public class CommentService : ICommentService
    {
        private readonly PostCommentsContext _context;

        public CommentService(PostCommentsContext context)
        {
            _context = context;
        }

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

        
        public async Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(int postId)
        {
            return await _context.Comments
                                 .Where(c => c.PostId == postId)
                                 .ToListAsync();
        }
        public async Task<IEnumerable<Comment>>GetCommentsByUserIdAsync(int userId){
            return await _context.Comments
            .Where(c=>c.UserId == userId)
            .ToListAsync();
        }


public async Task<Comment> UpdateCommentAsync(int postId, int commentId, UpdateCommentDto updatedCommentDto)
{
    if (updatedCommentDto == null || string.IsNullOrWhiteSpace(updatedCommentDto.Content))
    {
        throw new ArgumentException("Comment content is required.");
    }

    var existingComment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId && c.PostId == postId);
    if (existingComment == null)
    {
        throw new KeyNotFoundException($"Comment with ID {commentId} for Post ID {postId} not found.");
    }

    existingComment.Content = updatedCommentDto.Content;

    try
    {
        await _context.SaveChangesAsync(); 
        return existingComment;
    }
    catch (Exception ex)
    {
        throw new Exception("An error occurred while updating the comment.", ex);
    }
}

}
}
