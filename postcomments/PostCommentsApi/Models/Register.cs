namespace PostCommentsApi.Models
{
    public class Register
    {
        public int Id { get; set; }  
        public string Username { get; set; }  
        public string Password { get; set; } 
        public string Email { get; set; }  
        
        public ICollection<Post> Posts { get; set; }
        
        public ICollection<Comment> Comments { get; set; }
    }
}
