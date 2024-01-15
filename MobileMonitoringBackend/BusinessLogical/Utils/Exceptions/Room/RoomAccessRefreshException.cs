namespace MobileMonitoringBackend.BusinessLogical.Utils.Exceptions.Room;

public class RoomAccessRefreshException : MobileMonitoringException
{
    public RoomAccessRefreshException() 
        : base(StatusCodes.Status400BadRequest,
            ErrorCodes.RoomAccessRefresh,
            "You have access token for room different from the one in refesh token") {}
}