namespace MobileMonitoringBackend.BusinessLogical.Utils.Exceptions.Room;

public class RoomDoesNotExistException : MobileMonitoringException
{
    public RoomDoesNotExistException(string roomName) 
        : base(StatusCodes.Status404NotFound, 
            ErrorCodes.RoomDoesNotExist,
            $"Room {roomName} does not exist") {}
}