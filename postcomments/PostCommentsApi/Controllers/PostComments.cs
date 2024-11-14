using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; 
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

        public PostCommentsController(PostCommentsContext  context, IPostService postService, ICommentService commentService, IAuthService authService)
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

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var user = await _authService.AuthenticateAsync(loginRequest.Email, loginRequest.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            HttpContext.Session.SetInt32("UserId", user.Id);

            return Ok(new { message = "Login successful" });
        }
 [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] RegisterRequest registerRequest)
        {
            if (string.IsNullOrEmpty(registerRequest.Email) || 
                string.IsNullOrEmpty(registerRequest.Password) || 
                string.IsNullOrEmpty(registerRequest.Username))
            {
                return BadRequest("All fields are required.");
            }

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var checkEmailQuery = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
                var checkUsernameQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";

                using (var command = new MySqlCommand(checkEmailQuery, connection))
                {
                    command.Parameters.AddWithValue("@Email", registerRequest.Email);
                    var emailCount = Convert.ToInt32(await command.ExecuteScalarAsync());

                    if (emailCount > 0)
                    {
                        return Conflict("Email is already taken.");
                    }
                }

                using (var command = new MySqlCommand(checkUsernameQuery, connection))
                {
                    command.Parameters.AddWithValue("@Username", registerRequest.Username);
                    var usernameCount = Convert.ToInt32(await command.ExecuteScalarAsync());

                    if (usernameCount > 0)
                    {
                        return Conflict("Username is already taken.");
                    }
                }
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var insertQuery = "INSERT INTO Users (Username, Email, PasswordHash) VALUES (@Username, @Email, @PasswordHash)";
                using (var command = new MySqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Username", registerRequest.Username);
                    command.Parameters.AddWithValue("@Email", registerRequest.Email);
                    command.Parameters.AddWithValue("@PasswordHash", passwordHash);

                    await command.ExecuteNonQueryAsync();
                }
            }

            return Ok(new { message = "User registered successfully" });
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
