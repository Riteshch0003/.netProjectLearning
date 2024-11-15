namespace PostCommentsApi.Models
{
public class PostCreateDto
{
    public string Title { get; set; }
    public string Content { get; set; }
    public int UserId { get; set; }
}
}
