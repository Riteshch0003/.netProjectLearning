using Microsoft.AspNetCore.Mvc;
using PostCommentsApi.Models;
using PostCommentsApi.Services;
using PostCommentsApi.Data;
using Microsoft.EntityFrameworkCore;
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
        PostId = postId,  // Associate comment with the post
        UserId = commentDto.UserId  // Associate comment with the user
    };

    _context.Comments.Add(comment);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetComments), new { postId = postId }, comment);
}


      [HttpPost("login")]
public async Task<ActionResult> Login([FromBody] LoginRequest loginRequest)
{
    var user = await _authService.AuthenticateAsync(loginRequest.Email, loginRequest.Password);
    if (user == null)
    {
        return Unauthorized(new { message = "Invalid credentials" });
    }

    // Return the userId along with a success message
    return Ok(new { userId = user.Id, message = "Login successful" });
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
public async Task<ActionResult<Post>> EditPost(int userId, int postId, [FromBody] Post updatedPost)
{
    if (updatedPost == null || string.IsNullOrWhiteSpace(updatedPost.Title) || string.IsNullOrWhiteSpace(updatedPost.Content))
    {
        return BadRequest("Post title and content are required.");
    }

    // Find the post and validate the user
    var existingPost = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId && p.UserId == userId);
    if (existingPost == null)
    {
        return NotFound($"Post with ID {postId} for User ID {userId} not found.");
    }

    // Update post details
    existingPost.Title = updatedPost.Title;
    existingPost.Content = updatedPost.Content;

    try
    {
        await _context.SaveChangesAsync();
        return Ok(existingPost);
    }
    catch (Exception ex)
    {
        // Log the exception
        return StatusCode(500, "An error occurred while updating the post.");
    }
}
[HttpPut("post/{postId}/comments/{commentId}")]
public async Task<ActionResult<Comment>> EditComment(int postId, int commentId, [FromBody] Comment updatedComment)
{
    if (updatedComment == null || string.IsNullOrWhiteSpace(updatedComment.Content))
    {
        return BadRequest("Comment content is required.");
    }

    // Find the comment and validate it belongs to the specified post
    var existingComment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId && c.PostId == postId);
    if (existingComment == null)
    {
        return NotFound($"Comment with ID {commentId} for Post ID {postId} not found.");
    }

    // Update comment details
    existingComment.Content = updatedComment.Content;

    try
    {
        await _context.SaveChangesAsync();
        return Ok(existingComment);
    }
    catch (Exception ex)
    {
        // Log the exception
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
