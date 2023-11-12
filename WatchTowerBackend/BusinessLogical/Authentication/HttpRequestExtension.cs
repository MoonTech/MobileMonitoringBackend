using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace WatchTowerBackend.BusinessLogical.Authentication;

public static class HttpRequestExtension
{
    public static string? GetToken(this HttpRequest request)
    {
        var fullAuth = request.Headers.Authorization;
        var parsedAuth = AuthenticationHeaderValue.Parse(fullAuth);
        var accessToken = parsedAuth.Parameter;
        return accessToken;
    }
    
    public static string GetUserLoginFromToken(this HttpRequest request)
    {
        var token = request.GetToken();
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(token);
        var login = jwtSecurityToken.GetLogin();
        return login;
    }
}