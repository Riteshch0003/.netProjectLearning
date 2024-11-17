using Microsoft.EntityFrameworkCore;
using PostCommentsApi.Models;
using PostCommentsApi.Data;
using System;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace PostCommentsApi.Services
{
    public class AuthService : IAuthService
{
    private readonly PostCommentsContext _context;
    private readonly string _jwtSecret;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;

    public AuthService(PostCommentsContext context, IConfiguration configuration)
    {
        _context = context;
        _jwtSecret = configuration["Jwt:Key"];  // Fetch secret from configuration
        _jwtIssuer = configuration["Jwt:Issuer"];  // Fetch issuer from configuration
        _jwtAudience = configuration["Jwt:Audience"];  // Fetch audience from configuration
    }

    // Existing methods

        public async Task<User> AuthenticateAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || user.Password != password)  // Compare the plain text passwords
            {
                return null;
            }

            return user;
        }


        // Register a new user
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
                Password = password  // Store the plain text password
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser;
        }

        // Validate if the user exists (for registration)
        public async Task<bool> ValidateUserExistsAsync(string username, string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Username == username || u.Email == email);
        }

        // Generate JWT token for the authenticated user
        public string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Username),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

    