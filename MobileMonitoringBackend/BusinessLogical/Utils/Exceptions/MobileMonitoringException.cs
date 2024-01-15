using System.Net;
using System.Security.Cryptography.X509Certificates;
using SkiaSharp;

namespace MobileMonitoringBackend.BusinessLogical.Utils.Exceptions;

public class MobileMonitoringException : Exception
{
    public int HttpCode { get; }
    public string ErrorCode { get; }

    public MobileMonitoringException(int httpCode, string errorCode, string message) : base(message)
    {
        HttpCode = httpCode;
        ErrorCode = errorCode;
    }
}