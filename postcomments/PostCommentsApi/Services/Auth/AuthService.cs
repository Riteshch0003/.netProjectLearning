using PostCommentsApi.Models;
using PostCommentsApi.Data;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net; 

namespace PostCommentsApi.Services
{
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
                return null; 
            }

            return user; 
        }

        public async Task<User> RegisterAsync(string username, string email, string password)
        {
            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                throw new Exception("Email is already taken.");
            }

            if (await _context.Users.AnyAsync(u => u.Username == username))
            {
                throw new Exception("Username is already taken.");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var newUser = new User
            {
                Username = username,
                Email = email,
                PasswordHash = passwordHash
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser; 
        }

        public async Task<bool> ValidateUserExistsAsync(string username, string email)
        {
            var userExists = await _context.Users
                .AnyAsync(u => u.Username == username || u.Email == email);

            return userExists;  
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash); 
        }
    }
}
