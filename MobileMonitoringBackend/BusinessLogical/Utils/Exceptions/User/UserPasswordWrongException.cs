namespace MobileMonitoringBackend.BusinessLogical.Utils.Exceptions.User;

public class UserPasswordWrongException : MobileMonitoringException
{
    public UserPasswordWrongException(string userLogin)
        : base(StatusCodes.Status400BadRequest, 
            ErrorCodes.UserPasswordWrong, 
            $"Wrong password for user {userLogin}") {}
}