namespace MobileMonitoringBackend.BusinessLogical.Utils.Exceptions.Camera;

public class CameraAlreadyExistsException : MobileMonitoringException
{
    public CameraAlreadyExistsException(string message)
        : base(StatusCodes.Status400BadRequest, ErrorCodes.CameraAlreadyExists, message) {}
}