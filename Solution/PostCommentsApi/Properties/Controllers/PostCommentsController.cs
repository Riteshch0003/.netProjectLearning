using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostCommentsApi.Data;
using PostCommentsApi.Models;

namespace PostCommentsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PostsController(AppDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            var posts = await _context.Posts.Include(p => p.Comments).ToListAsync();
            return Ok(posts);
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            var post = await _context.Posts.Include(p => p.Comments)
                                           .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();  
            }

            return Ok(post);
        }

       
        [HttpPost]
        public async Task<ActionResult<Post>> PostPost(Post post)
        {
            _context.Posts.Add(post);  
            await _context.SaveChangesAsync();  
            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);  
        }

        [HttpPost("{postId}/comments")]
        public async Task<ActionResult<Comment>> PostComment(int postId, Comment comment)
        {
            var post = await _context.Posts.FindAsync(postId);

            if (post == null)
            {
                return NotFound(); 
            }

            comment.PostId = postId;
            _context.Comments.Add(comment);  
            await _context.SaveChangesAsync();  

            return CreatedAtAction(nameof(GetPost), new { id = postId }, comment);  
        }
    }
}
