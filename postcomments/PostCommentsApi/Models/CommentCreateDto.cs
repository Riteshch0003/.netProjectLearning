namespace PostCommentsApi.Models
{
    public class CommentCreateDto
    {
        public string Content { get; set; }    
        public string Author { get; set; }   
        public int PostId { get; set; }     
        public int UserId { get; set; }   

    }
}
