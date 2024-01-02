
namespace WatchTowerBackend.BusinessLogical.Utils.Exceptions;

public class ObjectAlreadyExistsInDbException : Exception
{
    public ObjectAlreadyExistsInDbException(string message) : base(message) {}
}