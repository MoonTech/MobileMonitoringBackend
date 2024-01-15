namespace MobileMonitoringBackend.BusinessLogical.Utils.Exceptions.User;

public class UserDoesNotExistException : MobileMonitoringException
{
    public UserDoesNotExistException(string userLogin)
        : base(StatusCodes.Status404NotFound, 
            ErrorCodes.UserDoesNotExist, 
            $"User {userLogin} does not exist") {}
}