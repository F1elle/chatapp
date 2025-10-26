namespace ChatApp.Auth.Infrastructure.Security;


public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password) =>
        BCrypt.Net.BCrypt.EnhancedHashPassword(password);

    public bool VerifyPassword(string password, string hash) =>
        BCrypt.Net.BCrypt.EnhancedVerify(password, hash);
    
}