namespace MobileMonitoringBackend.BusinessLogical.Utils.Exceptions.VideoServer;

public class VideoServerCommunicationException : MobileMonitoringException
{
    public VideoServerCommunicationException(int statusCode, string message)
        : base(statusCode,
            ErrorCodes.VideoServerCommunication,
            message) { }
}