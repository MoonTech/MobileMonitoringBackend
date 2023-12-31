namespace WatchTowerBackend.BusinessLogical.Repositories.VideoServerRepository;

public interface IVideoServerRepository
{
    public Task<HttpResponseMessage?> StartRecording(string endpoint);
    public Task<HttpResponseMessage?> StopRecording(string endpoint);
    public Task<HttpResponseMessage?> GetRecording(string endpoint);
    public Task<HttpResponseMessage?> DeleteRecording(string endpoint);
}