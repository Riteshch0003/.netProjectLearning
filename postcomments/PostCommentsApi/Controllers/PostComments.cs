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
        private readonly IAuthService _authService;

        public PostCommentsController(IPostService postService, ICommentService commentService, IAuthService AuthService)
        {
            _postService = postService;
            _commentService = commentService;
            _authService = AuthService;
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
        public async Task<ActionResult<Post>> PostPost([FromBody] Post post)
        {
            var createdPost = await _postService.CreatePostAsync(post);
            return CreatedAtAction(nameof(GetPost), new { id = createdPost.Id }, createdPost);
        }

       [HttpPost("{postId}/comments")]
    public async Task<ActionResult<Comment>> AddComment(int postId, Comment comment)
    {
        try
        {
            var createdComment = await _commentService.AddCommentAsync(postId, comment);
            return CreatedAtAction(nameof(GetComments), new { postId = postId }, createdComment);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Post not found.");
        }
    }

    [HttpGet("{postId}/comments")]
    public async Task<ActionResult<IEnumerable<Comment>>> GetComments(int postId)
    {
        var comments = await _commentService.GetCommentsByPostIdAsync(postId);
        return Ok(comments);
    }
// Login API endpoint
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var user = await _authService.AuthenticateAsync(loginRequest.Email, loginRequest.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            // Store user ID in session
            HttpContext.Session.SetInt32("UserId", user.Id);

            return Ok(new { message = "Login successful" });
        }
    }
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

