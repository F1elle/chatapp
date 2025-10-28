namespace ChatApp.Auth.Infrastructure.Security;


public class PasswordHasher 
{
    public string HashPassword(string password) =>
        BCrypt.Net.BCrypt.EnhancedHashPassword(password);

    public bool VerifyPassword(string password, string hash) =>
        BCrypt.Net.BCrypt.EnhancedVerify(password, hash);
    
}