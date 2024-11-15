using Microsoft.AspNetCore.Mvc;
using PostCommentsApi.Models;
using PostCommentsApi.Services;
using PostCommentsApi.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using MySql.Data.MySqlClient;

namespace PostCommentsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostCommentsController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly PostCommentsContext _context;
        private readonly IPostService _postService;
        private readonly ICommentService _commentService;
        private readonly IAuthService _authService;

        public PostCommentsController(PostCommentsContext context, IPostService postService, ICommentService commentService, IAuthService authService)
        {
            _context = context;
            _postService = postService;
            _commentService = commentService;
            _authService = authService;
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

        [HttpGet("{postId}/comments")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComments(int postId)
        {
            var comments = await _commentService.GetCommentsByPostIdAsync(postId);
            return Ok(comments);
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

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var user = await _authService.AuthenticateAsync(loginRequest.Email, loginRequest.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }
            return Ok(new { message = "Login successful" });
        }

        // Register method for new users
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            if (string.IsNullOrEmpty(registerRequest.Email) || 
                string.IsNullOrEmpty(registerRequest.Password) || 
                string.IsNullOrEmpty(registerRequest.Username))
            {
                return BadRequest("All fields are required.");
            }

            try
            {
                var newUser = await _authService.RegisterAsync(registerRequest.Username, registerRequest.Email, registerRequest.Password);
                return Ok(new { message = "User registered successfully", user = newUser });
            }
            catch (Exception ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
        [HttpGet("user/{userId}")]
        public async Task<ActionResult> GetPostsWithUserComments(int userId)
        {
            var posts = await _postService.GetPostsByUserIdAsync(userId);
            if (posts == null || !posts.Any())
                return NotFound("No posts found for this user.");

            var response = new List<object>();

            foreach (var post in posts)
            {
                var comments = await _commentService.GetCommentsByUserIdAsync(userId);
                var userComments = comments.Where(c => c.UserId == userId).ToList();

                response.Add(new
                {
                    Id = post.Id,
                    Title = post.Title,
                    Content = post.Content,
                    Comments = userComments.Select(c => new
                    {
                        Id = c.Id,
                        Author = c.Author,
                        Content = c.Content,
                        UserId = c.UserId,
                        PostId = c.PostId
                    })
                });
            }

            return Ok(response);
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
