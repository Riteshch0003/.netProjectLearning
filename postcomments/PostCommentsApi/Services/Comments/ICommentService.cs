using PostCommentsApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PostCommentsApi.Services
{
    public interface ICommentService
    {
        Task<Comment> AddCommentAsync(int postId, Comment comment);
        Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(int postId);
        Task<IEnumerable<Comment>> GetCommentsByUserIdAsync(int userId);
        Task<Comment> UpdateCommentAsync(int postId, int commentId, UpdateCommentDto updatedCommentDto);

    }
}
