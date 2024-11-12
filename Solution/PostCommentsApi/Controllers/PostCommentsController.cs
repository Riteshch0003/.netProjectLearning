using Microsoft.AspNetCore.Mvc;
using PostCommentsApi.Models;
using PostCommentsApi.Services;

namespace PostCommentsApi.Controllers
{
    [ApiController]
[Route("api/[controller]")]
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

    // Endpoint to create a new post
  [HttpPost("posts")]
public async Task<ActionResult<Post>> PostPost(Post post)
{
    if (post.Comments == null || post.Comments.Count == 0)
    {
        return BadRequest("The comment field is required.");
    }
    var createdPost = await _postService.CreatePostAsync(post);
    return CreatedAtAction(nameof(GetPost), new { id = createdPost.Id }, createdPost);
}

    // Endpoint to create a new comment
    [HttpPost("comments")]
    public async Task<ActionResult<Comment>> PostComment(Comment comment)
    {
        try
        {
            var createdComment = await _postService.CreateCommentAsync(comment);
            return CreatedAtAction(nameof(GetPost), new { id = createdComment.PostId }, createdComment);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
 }
}
