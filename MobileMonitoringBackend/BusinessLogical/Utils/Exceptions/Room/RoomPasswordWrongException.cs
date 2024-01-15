namespace MobileMonitoringBackend.BusinessLogical.Utils.Exceptions.Room;

public class RoomPasswordWrongException : MobileMonitoringException
{
    public RoomPasswordWrongException(string roomName)
        : base(StatusCodes.Status400BadRequest, 
            ErrorCodes.RoomPasswordWrong, 
            $"Wrong password for room {roomName}") {}
}