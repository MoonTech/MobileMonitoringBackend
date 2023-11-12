using System.IdentityModel.Tokens.Jwt;

namespace WatchTowerBackend.BusinessLogical.Authentication;

public static class JwtSecurityTokenExtension
{
    public static string GetLogin(this JwtSecurityToken jwtSecurityToken)
    {
        return jwtSecurityToken.Claims.First(claim => claim.Type == "login").Value;
    }
}