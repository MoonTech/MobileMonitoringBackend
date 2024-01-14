namespace MobileMonitoringBackend.BusinessLogical.Utils.Exceptions.Recording;

public class RecordingDoesNotExistException : MobileMonitoringException
{
    public RecordingDoesNotExistException(string recordingName)
        : base(StatusCodes.Status404NotFound,
            ErrorCodes.RecordingDoesNotExist,
            $"Recording {recordingName} doesn't exist") {}
}