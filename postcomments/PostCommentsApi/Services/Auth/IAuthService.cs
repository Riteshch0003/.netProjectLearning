using PostCommentsApi.Models;
using System.Threading.Tasks;

namespace PostCommentsApi.Services
{
    public interface IAuthService
    {
        Task<User> AuthenticateAsync(string email, string password);  
        Task<User> RegisterAsync(string username, string email, string password);  
        Task<bool> ValidateUserExistsAsync(string username, string email);  
        string GenerateJwtToken(User user);
    }
}
