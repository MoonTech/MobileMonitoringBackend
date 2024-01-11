namespace MobileMonitoringBackend.BusinessLogical.Utils.Exceptions;

public class ObjectDoesNotExistInDbException : Exception
{
    public ObjectDoesNotExistInDbException(string message) : base(message) {}
}