namespace WatchTowerBackend.BusinessLogical.Utils;

public static class Constants
{
    public static string StreamBaseUrl = "rtmp://10.0.2.2:1935/live/";
    public static string WatchBaseUrl = "http://localhost:8080/hls/";
    public static string RecordBaseUrl = "http://localhost:8080/";
    public static string ApiHttpUrl = "http://localhost:5266/";
    public static string StartRecordingEndpoint(string cameraToken)
    {
        return $"control/record/start?app=live&name={cameraToken}&rec=rec1";
    }
    public static string StopRecordingEndpoint(string cameraToken)
    {
        return $"control/record/stop?app=live&name={cameraToken}&rec=rec1";
    }
    // TODO Add constants for [Authorize] attribute
}