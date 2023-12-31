namespace WatchTowerBackend.BusinessLogical.Utils.Exceptions;

public class CouldNotSaveChangesException : Exception
{
    public CouldNotSaveChangesException(string message) : base(message) {}
}