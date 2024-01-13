namespace MobileMonitoringBackend.BusinessLogical.Utils.Exceptions.User;

public class UserAlreadyExistsException : MobileMonitoringException
{
    public UserAlreadyExistsException(string userLogin) 
        : base(StatusCodes.Status400BadRequest, 
            ErrorCodes.UserAlreadyExists,
            $"User {userLogin} already exists") {}
}