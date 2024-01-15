namespace MobileMonitoringBackend.BusinessLogical.Utils.Exceptions.Camera;

public class CameraAlreadyAcceptedException : MobileMonitoringException
{
    public CameraAlreadyAcceptedException(Guid id)
        : base(StatusCodes.Status405MethodNotAllowed,
            ErrorCodes.CameraAlreadyAccepted,
            $"Camera {id} has been already accepted") {}
}