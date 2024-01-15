namespace MobileMonitoringBackend.BusinessLogical.Utils.Exceptions;

public class CouldNotSaveChangesException : MobileMonitoringException
{
    public CouldNotSaveChangesException()
        : base(StatusCodes.Status500InternalServerError,
            ErrorCodes.CouldNotSaveChanges,
            "Could not save changes") {}
}