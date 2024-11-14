using PostCommentsApi.Models;  // For User model
using PostCommentsApi.Data;    // For PostCommentsContext
using PostCommentsApi.Services;  // For IAuthService interface
using Microsoft.EntityFrameworkCore;


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
        return password == storedHash; // For simplicity; use bcrypt in production
    }
}
