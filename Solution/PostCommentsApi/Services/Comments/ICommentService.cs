// Services/ICommentService.cs
using PostCommentsApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PostCommentsApi.Services
{
    public interface ICommentService
    {
        Task<Comment> AddCommentAsync(int postId, Comment comment);
        Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(int postId);
    }
}
