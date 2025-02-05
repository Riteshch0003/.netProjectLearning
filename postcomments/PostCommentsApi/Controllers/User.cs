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
        // get all posts
        [AllowAnonymous]
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
        // get posts according to the id 
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
        // add posts by the logged in user wants
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
            // get all comments by the user if wats to
        [HttpGet("{postId}/comments")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComments(int postId)
        {
            var comments = await _commentService.GetCommentsByPostIdAsync(postId);
            return Ok(comments);
        }
        // the comments being add by the user who is already logged in
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
            // user login with jwt barear token 
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            Console.WriteLine($"[Login] Received login request for email: {loginRequest.Email}");

            if (string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
            {
                Console.WriteLine("[Login] Email or password is missing.");
                return BadRequest("Email and password are required.");
            }

            // Try to authenticate the user
            var user = await _authService.AuthenticateAsync(loginRequest.Email, loginRequest.Password);

            if (user == null)
            {
                Console.WriteLine($"[Login] Authentication failed for email: {loginRequest.Email}");
                return Unauthorized(new { message = "Invalid email or password" });
            }

            // Log the successful user authentication
            Console.WriteLine($"[Login] User authenticated: {user.Email}");

            // Generate the JWT token
            var token = _authService.GenerateJwtToken(user);
            Console.WriteLine("[Login] JWT token generated for user: " + user.Email);

            return Ok(new { message = "Login successful", token, userId = user.Id });
        }

        
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { message = "Logout successful, please clear the token on the client side." });
        }

        // Helper method to generate JWT token
        // This method generates a JWT token for the authenticated user.
        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Name, user.Username),
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisIsASecretKeyForJwtAuthenticationThatIsLongEnough123"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "PostCommentsApi",
                audience: "PostCommentsApi",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );
            
            return new JwtSecurityTokenHandler().WriteToken(token);

        }

            // register new user with hashed password
        [HttpPost("register")]
        [AllowAnonymous]
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
            // get posts of the logged user nd with comments they had done 
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
        // edit the posts if logged in user wants too 
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