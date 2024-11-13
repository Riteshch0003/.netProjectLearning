namespace PostCommentsApi.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        
        [Required]
    public string CommentText { get; set; }
    
    [Required]
    public string Author { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    [Required]
    public int PostId { get; set; }
    }
}
