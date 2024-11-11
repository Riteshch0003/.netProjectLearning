using Microsoft.AspNetCore.Mvc;
using PostCommentsApi.Models;
using PostCommentsApi.Services;

namespace PostCommentsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostCommentsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostCommentsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            var posts = await _postService.GetPostsAsync();
            return Ok(posts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            var post = await _postService.GetPostByIdAsync(id);

            if (post == null)
                return NotFound();

            return Ok(post);
        }

        [HttpPost]
        public async Task<ActionResult<Post>> PostPost(Post post)
        {
            var createdPost = await _postService.CreatePostAsync(post);
            return CreatedAtAction(nameof(GetPost), new { id = createdPost.Id }, createdPost);
        }

        [HttpPost("{postId}/comments")]
        public async Task<ActionResult<Comment>> PostComment(int postId, Comment comment)
        {
            var createdComment = await _postService.AddCommentToPostAsync(postId, comment);

            if (createdComment == null)
                return NotFound(new { Message = "Post not found" });

            // Return the created comment with proper route to get it by postId and comment Id
            return CreatedAtAction(nameof(PostComment), new { postId = postId, id = createdComment.Id }, createdComment);
        }
    }
}
