using PostCommentsApi.Models;  // For User model
using PostCommentsApi.Data;    // For PostCommentsContext
using Microsoft.EntityFrameworkCore;

namespace PostCommentsApi.Services
{
    public interface IAuthService
    {
        Task<User> AuthenticateAsync(string email, string password);
    }

    public class AuthService : IAuthService
    {
        private readonly PostCommentsContext _context;

        public AuthService(PostCommentsContext context)
        {
            _context = context;
        }

        public async Task<User> AuthenticateAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || !VerifyPassword(password, user.PasswordHash))
            {
                return null; // Invalid credentials
            }

            return user; // Return the authenticated user
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            // Use a password hashing algorithm like bcrypt or PBKDF2 for production
            return password == storedHash; // Just for simplicity, avoid in production
        }
    }
}
