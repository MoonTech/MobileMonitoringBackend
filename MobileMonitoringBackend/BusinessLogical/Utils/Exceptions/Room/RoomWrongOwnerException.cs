namespace MobileMonitoringBackend.BusinessLogical.Utils.Exceptions.Room;

public class RoomWrongOwnerException : MobileMonitoringException
{
    public RoomWrongOwnerException()
        : base(StatusCodes.Status401Unauthorized,
            ErrorCodes.RoomWrongOwner,
            "You are not an owner of this room") {}
}