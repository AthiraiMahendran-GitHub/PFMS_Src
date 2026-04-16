using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Data;
using System.Security.Cryptography;
using System.Text;

namespace PersonalFinanceManager.Services;

public class UserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> RegisterUserAsync(string fullName, string email, string username, string password)
    {
        // Check if email already exists
        if (await _context.Users.AnyAsync(u => u.Email == email))
        {
            return null; // Email already exists
        }

        // Check if username already exists
        if (await _context.Users.AnyAsync(u => u.Username == username))
        {
            return null; // Username already exists
        }

        var user = new User
        {
            FullName = fullName,
            Email = email,
            Username = username,
            PasswordHash = HashPassword(password),
            IsActive = true,
            CreatedAt = DateTime.Now
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User?> LoginAsync(string usernameOrEmail, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => 
                (u.Username == usernameOrEmail || u.Email == usernameOrEmail) 
                && u.IsActive);

        if (user == null)
        {
            return null;
        }

        if (!VerifyPassword(password, user.PasswordHash))
        {
            return null;
        }

        // Update last login
        user.LastLoginAt = DateTime.Now;
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private bool VerifyPassword(string password, string passwordHash)
    {
        var hashedInput = HashPassword(password);
        return hashedInput == passwordHash;
    }
}
