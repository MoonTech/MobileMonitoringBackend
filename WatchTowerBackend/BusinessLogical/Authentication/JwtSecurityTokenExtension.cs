using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WatchTowerBackend.BusinessLogical.Authentication;

public static class JwtSecurityTokenExtension
{
    public static string? GetClaim(this JwtSecurityToken jwtSecurityToken, string claimType)
    {
        var claim = jwtSecurityToken.Claims.SingleOrDefault(claim => claim.Type == claimType);
        return claim is not null ? claim.Value : null;
    }

    public static string GenerateToken(IConfiguration config, string apiKeyPath, Claim[] claims)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config[apiKeyPath]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(config["Jwt:Issuer"],
            config["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentials);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}