using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace MobileMonitoringBackend.BusinessLogical.Authentication;

public static class HttpRequestExtension
{
    public static string? GetToken(this HttpRequest request)
    {
        var fullAuth = request.Headers.Authorization;
        var parsedAuth = AuthenticationHeaderValue.Parse(fullAuth);
        var accessToken = parsedAuth.Parameter;
        return accessToken;
    }

    public static string GetClaimFromToken(this HttpRequest request, string claimType)
    {
        var token = request.GetToken();
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(token);
        var claim = jwtSecurityToken.GetClaim(claimType);
        return claim;
    }
    
    public static string GetUserLoginFromToken(this HttpRequest request)
    {
        return request.GetClaimFromToken("login");
    }
    
    public static string GetRoomNameFromToken(this HttpRequest request)
    {
        return request.GetClaimFromToken("RoomName");
    }
}