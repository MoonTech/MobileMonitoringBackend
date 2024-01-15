namespace MobileMonitoringBackend.BusinessLogical.Utils.Exceptions.Recording;

public class RecordingCamerasCacheException : MobileMonitoringException
{
    public RecordingCamerasCacheException()
        : base(StatusCodes.Status500InternalServerError,
            ErrorCodes.RecordingCamerasCache,
            "Internal server error concerning recording cameras cache") {}
}