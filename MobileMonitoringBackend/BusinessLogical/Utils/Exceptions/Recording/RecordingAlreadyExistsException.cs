namespace MobileMonitoringBackend.BusinessLogical.Utils.Exceptions.Recording;

public class RecordingAlreadyExistsException : MobileMonitoringException
{
    public RecordingAlreadyExistsException(string recordingName)
        :base(StatusCodes.Status400BadRequest,
            ErrorCodes.RecordingAlreadyExists,
            $"Recording {recordingName} already exists") {}
}