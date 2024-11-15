using PostCommentsApi.Models;
namespace PostCommentsApi.Services
{
    public interface IPostService
    {
        Task<IEnumerable<Post>> GetPostsAsync();
        Task<Post> GetPostByIdAsync(int id);
        Task<Post> CreatePostAsync(Post post);
        Task<IEnumerable<Post>> GetPostsByUserIdAsync(int userId);
        Task<Post> AddPost(int userId, Post post);
        Task<Post> UpdatePostAsync(int userId, int postId, Post updatedPost);
    }
}
