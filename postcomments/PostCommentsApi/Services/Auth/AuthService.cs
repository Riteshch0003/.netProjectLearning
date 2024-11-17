using Microsoft.EntityFrameworkCore;
using PostCommentsApi.Models;
using PostCommentsApi.Data;
using System;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Security.Claims; 
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

       Console.WriteLine($"JWT Configuration - Issuer: {_jwtIssuer}, Audience: {_jwtAudience}, Secret: {_jwtSecret}");
    }

public static string HashPassword(string password)
{
    // Log the password hashing process
    Console.WriteLine($"Hashing password: {password}");

    using (var sha256 = SHA256.Create())
    {
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        string hashedPassword = Convert.ToBase64String(hash);
        
        // Log the hashed password
        Console.WriteLine($"Generated hashed password: {hashedPassword}");
        return hashedPassword;
    }
}



public async Task<User> AuthenticateAsync(string email, string password)
{
    // Log the email check
    Console.WriteLine($"[AuthService] Checking if user exists with email: {email}");

    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    if (user == null)
    {
        // Log when the user is not found
        Console.WriteLine($"[AuthService] User not found with email: {email}");
        return null;
    }

    // Log user retrieval success
    Console.WriteLine($"[AuthService] User found: {user.Email}");

    // Hash the provided password and log it
    string hashedPassword = HashPassword(password);
    Console.WriteLine($"[AuthService] Hashed input password: {hashedPassword}");
    Console.WriteLine($"[AuthService] Stored password hash: {user.Password}");

    // Compare the provided password with the stored hashed password
    if (user.Password != hashedPassword)
    {
        // Log password mismatch
        Console.WriteLine($"[AuthService] Password mismatch for email: {email}");
        return null;
    }

    // Log successful password match
    Console.WriteLine($"[AuthService] Password matched for user: {email}");

    return user;
}




        // Register a new user
      public async Task<User> RegisterAsync(string username, string email, string password)
{
    // Check if the email already exists
    var emailExists = await _context.Users.AnyAsync(u => u.Email == email);
    if (emailExists)
    {
        throw new Exception("Email is already taken.");
    }

    // Check if the username already exists
    var usernameExists = await _context.Users.AnyAsync(u => u.Username == username);
    if (usernameExists)
    {
        throw new Exception("Username is already taken.");
    }

    // Hash the password before storing it
    var hashedPassword = HashPassword(password);

    // Create the new user object
    var newUser = new User
    {
        Username = username,
        Email = email,
        Password = hashedPassword  // Store the hashed password
    };

    // Add the new user to the database
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
    var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Name, user.Username),
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisIsASecretKeyForJwtAuthenticationThatIsLongEnough123"));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var token = new JwtSecurityToken(
        issuer: "PostCommentsApi",
        audience: "PostCommentsApi",
        claims: claims,
        expires: DateTime.Now.AddHours(1),
        signingCredentials: creds
    );

    var jwt = new JwtSecurityTokenHandler().WriteToken(token);
    Console.WriteLine("Generated JWT: " + jwt);  // Log the generated token
    return jwt;
}

    }
}

    