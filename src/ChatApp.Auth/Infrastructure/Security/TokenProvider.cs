using System.Text;
using ChatApp.Auth.Domain;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace ChatApp.Auth.Infrastructure.Security;


public class TokenProvider(IOptions<JwtOptions> jwtOptions)
{
    private readonly IOptions<JwtOptions> _options = jwtOptions;
    public string Create(UserAuth userAuth)
    {
        Claim[] claims = [
            new(JwtRegisteredClaimNames.Sub, userAuth.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, userAuth.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        var securityKey = SecurityKey(_options.Value.Secret);

        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Value.Issuer,
            audience: _options.Value.Audience,
            signingCredentials: signingCredentials,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_options.Value.ExpirationInMinutes));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string CreateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private static SymmetricSecurityKey SecurityKey(string key) => new(Encoding.UTF8.GetBytes(key));
}