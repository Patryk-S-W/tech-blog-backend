using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TechBlog.Application.Common;
using TechBlog.Domain.Users;

namespace TechBlog.Infrastructure.Services;

public sealed class JwtTokenGenerator(IConfiguration configuration) : ITokenGenerator
{
    public string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username.Value),
        };

        var appSettingsToken = configuration.GetSection("AppSettings:Token").Value
            ?? throw new InvalidOperationException("AppSettings:Token is not configured.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettingsToken));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = credentials,
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
