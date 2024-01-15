namespace MobileMonitoringBackend.BusinessLogical.Utils.Exceptions.Camera;

public class CameraDoesNotExistException : MobileMonitoringException
{
    public CameraDoesNotExistException(string message)
        : base(StatusCodes.Status404NotFound, ErrorCodes.CameraDoesNotExist, message) {}
}