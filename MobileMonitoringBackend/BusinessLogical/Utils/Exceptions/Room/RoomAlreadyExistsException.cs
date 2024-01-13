namespace MobileMonitoringBackend.BusinessLogical.Utils.Exceptions.Room;

public class RoomAlreadyExistsException : MobileMonitoringException
{
    public RoomAlreadyExistsException(string roomName) 
        : base(StatusCodes.Status400BadRequest, 
            ErrorCodes.RoomAlreadyExists,
            $"Room {roomName} already exists") {}
}