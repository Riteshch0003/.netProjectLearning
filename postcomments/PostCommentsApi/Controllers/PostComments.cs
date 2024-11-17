using Microsoft.AspNetCore.Mvc;
using PostCommentsApi.Models;
using PostCommentsApi.Services;
using PostCommentsApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostCommentsApi.Controllers
{
    [Authorize] 
    [Route("api/[controller]")]
    [ApiController]
    public class PostCommentsController : ControllerBase
    {
        private readonly PostCommentsContext _context;
        private readonly IPostService _postService;
        private readonly ICommentService _commentService;
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        // Constructor with dependency injection for services and IConfiguration
        public PostCommentsController(PostCommentsContext context, IPostService postService, ICommentService commentService, IAuthService authService, IConfiguration configuration)
        {
            _context = context;
            _postService = postService;
            _commentService = commentService;
            _authService = authService;
            _configuration = configuration;  
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            var posts = await _context.Posts.ToListAsync();
            return Ok(posts);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetAllPosts()
        {
            var posts = await _postService.GetAllPosts();
            if (posts == null || !posts.Any())
            {
                return NotFound();
            }
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

        [HttpPost("user/{userId}")]
        public async Task<ActionResult<Post>> AddPost(int userId, [FromBody] PostCreateDto postDto)
        {
            if (string.IsNullOrWhiteSpace(postDto.Title) || string.IsNullOrWhiteSpace(postDto.Content))
            {
                return BadRequest(new { error = "Title and content are required." });
            }

            var post = new Post
            {
                Title = postDto.Title,
                Content = postDto.Content,
                UserId = userId
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
        }

        [HttpGet("{postId}/comments")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComments(int postId)
        {
            var comments = await _commentService.GetCommentsByPostIdAsync(postId);
            return Ok(comments);
        }

        [HttpPost("{postId}/comments")]
        public async Task<ActionResult<Comment>> AddComment(int postId, [FromBody] CommentCreateDto commentDto)
        {
            if (string.IsNullOrWhiteSpace(commentDto.Content) || string.IsNullOrWhiteSpace(commentDto.Author))
            {
                return BadRequest(new { error = "Content and author are required." });
            }

            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
            {
                return NotFound(new { error = "Post not found." });
            }

            var comment = new Comment
            {
                Content = commentDto.Content,
                Author = commentDto.Author,
                PostId = postId,  
                UserId = commentDto.UserId  
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetComments), new { postId = postId }, comment);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var secretKey = _configuration["Jwt:Key"]; 
            var issuer = _configuration["Jwt:Issuer"]; 
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginRequest.Email && u.Password == loginRequest.Password);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            var token = GenerateJwtToken(user, secretKey, issuer);
            
            return Ok(new { message = "Login successful", token });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { message = "Logout successful, please clear the token on the client side." });
        }

        // Helper method to generate JWT token
        private string GenerateJwtToken(User user, string secretKey, string issuer)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Username),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)); 
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); 
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: issuer, 
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token); 
        }

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

        [HttpPut("user/{userId}/post/{postId}")]
        public async Task<ActionResult<Post>> EditPost(int userId, int postId, [FromBody] UpdatePostDto updatedPostDto)
        {
            if (updatedPostDto == null || string.IsNullOrWhiteSpace(updatedPostDto.Title) || string.IsNullOrWhiteSpace(updatedPostDto.Content))
            {
                return BadRequest("Post title and content are required.");
            }

            var existingPost = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId && p.UserId == userId);
            if (existingPost == null)
            {
                return NotFound($"Post with ID {postId} for User ID {userId} not found.");
            }

            existingPost.Title = updatedPostDto.Title;
            existingPost.Content = updatedPostDto.Content;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(existingPost);
            }
            catch (Exception ex)
            {        
                return StatusCode(500, "An error occurred while updating the post.");
            }
        }

        [HttpPut("post/{postId}/comments/{commentId}")]
        public async Task<ActionResult<Comment>> EditComment(int postId, int commentId, [FromBody] UpdateCommentDto updatedCommentDto)
        {
            if (updatedCommentDto == null || string.IsNullOrWhiteSpace(updatedCommentDto.Content))
            {
                return BadRequest("Comment content is required.");
            }

            var existingComment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId && c.PostId == postId);
            if (existingComment == null)
            {
                return NotFound($"Comment with ID {commentId} for Post ID {postId} not found.");
            }

            existingComment.Content = updatedCommentDto.Content;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(existingComment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the comment.");
            }
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