using Microsoft.AspNetCore.Mvc;
using PostCommentsApi.Models;
using PostCommentsApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PostCommentsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostCommentsController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly ICommentService _commentService;

        public PostCommentsController(IPostService postService, ICommentService commentService)
        {
            _postService = postService;
            _commentService = commentService;
        }

        // Get all posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            var posts = await _postService.GetPostsAsync();
            return Ok(posts);
        }

        // Get a single post by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null)
                return NotFound();

            return Ok(post);
        }

        // Create a new post
        [HttpPost]
        public async Task<ActionResult<Post>> PostPost([FromBody] Post post)
        {
            var createdPost = await _postService.CreatePostAsync(post);
            return CreatedAtAction(nameof(GetPost), new { id = createdPost.Id }, createdPost);
        }

        // Add a comment to a specific post
       [HttpPost("{postId}/comments")]
    public async Task<ActionResult<Comment>> AddComment(int postId, Comment comment)
    {
        try
        {
            // Add the comment for the given postId
            var createdComment = await _commentService.AddCommentAsync(postId, comment);
            return CreatedAtAction(nameof(GetComments), new { postId = postId }, createdComment);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Post not found.");
        }
    }

    // Endpoint to get all comments for a specific post
    [HttpGet("{postId}/comments")]
    public async Task<ActionResult<IEnumerable<Comment>>> GetComments(int postId)
    {
        var comments = await _commentService.GetCommentsByPostIdAsync(postId);
        return Ok(comments);
    }
}
}
