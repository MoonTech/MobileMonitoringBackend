namespace MobileMonitoringBackend.BusinessLogical.Utils.Exceptions;

public class MobileMonitoringExceptionJSON
{
    public string ErrorCode { get; }
    public string Message { get; }

    public MobileMonitoringExceptionJSON(MobileMonitoringException ex)
    {
        ErrorCode = ex.ErrorCode;
        Message = ex.Message;
    }
}