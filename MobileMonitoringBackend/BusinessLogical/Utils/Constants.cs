using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MobileMonitoringBackend.BusinessLogical.Utils;

public static class Constants

{
    public static readonly string VideoServerHost = "161.35.206.64";

    public static string StreamBaseUrl = $"rtmp://{VideoServerHost}:1936/live/";
    public static string WatchBaseUrl = $"http://{VideoServerHost}:8081/live/";
    public static string RecordBaseUrl = $"http://{VideoServerHost}:8080/";

    public static string StartRecordingEndpoint(string cameraToken)
    {
        return $"control/record/start?app=live&name={cameraToken}&rec=rec1";
    }

    public static string StopRecordingEndpoint(string cameraToken)
    {
        return $"control/record/stop?app=live&name={cameraToken}&rec=rec1";
    }

    public static TokenValidationParameters TokenValidationParameters(
        IConfiguration config, string signingKeyAppsettingsKey)
    {
        return new TokenValidationParameters()
        {
            ClockSkew = TimeSpan.Zero,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config[signingKeyAppsettingsKey]))
        };
    }
    // TODO Add constants for [Authorize] attribute
}