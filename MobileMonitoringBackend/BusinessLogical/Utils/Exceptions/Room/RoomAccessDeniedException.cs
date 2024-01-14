namespace MobileMonitoringBackend.BusinessLogical.Utils.Exceptions.Room;

public class RoomAccessDeniedException : MobileMonitoringException
{
    public RoomAccessDeniedException(string roomName) 
        : base (StatusCodes.Status403Forbidden,
            ErrorCodes.RoomAccessDenied,
            $"You don't have permission to access room {roomName}") {}
}