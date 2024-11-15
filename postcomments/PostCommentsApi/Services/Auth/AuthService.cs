using Microsoft.EntityFrameworkCore;
using PostCommentsApi.Models;
using PostCommentsApi.Data;
using System;
using System.Threading.Tasks;

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
    if (user == null || user.Password != password) 
    {
        return null;
    }

    return user;
}



        public async Task<User> RegisterAsync(string username, string email, string password)
        {
            var emailExists = await _context.Users.AnyAsync(u => u.Email == email);
            if (emailExists)
            {
                throw new Exception("Email is already taken.");
            }
            var usernameExists = await _context.Users.AnyAsync(u => u.Username == username);
            if (usernameExists)
            {
                throw new Exception("Username is already taken.");
            }
            var newUser = new User
            {
                Username = username,
                Email = email,
                Password = password 
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser;
        }

        public async Task<bool> ValidateUserExistsAsync(string username, string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Username == username || u.Email == email);
        }
    }
}
