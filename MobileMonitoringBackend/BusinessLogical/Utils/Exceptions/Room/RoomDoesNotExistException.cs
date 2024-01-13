namespace MobileMonitoringBackend.BusinessLogical.Utils.Exceptions.Room;

public class RoomDoesNotExistException : MobileMonitoringException
{
    public RoomDoesNotExistException(string roomName) 
        : base(StatusCodes.Status400BadRequest, 
            ErrorCodes.RoomDoesNotExist,
            $"Room {roomName} does not exist") {}
}