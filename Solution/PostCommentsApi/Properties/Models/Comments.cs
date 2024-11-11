namespace PostCommentsApi.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string CommentText { get; set; }
        public string Author { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Post Post { get; set; }
    }
}
