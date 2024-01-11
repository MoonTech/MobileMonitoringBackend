namespace MobileMonitoringBackend.BusinessLogical.Repositories.VideoServerRepository;

public class VideoServerRepository : IVideoServerRepository
{
    private readonly HttpClient _httpClient;

    public VideoServerRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<HttpResponseMessage?> StartRecording(string endpoint)
    {
        return await _httpClient.GetAsync(endpoint);
    }

    public async Task<HttpResponseMessage?> StopRecording(string endpoint)
    {
        return await _httpClient.GetAsync(endpoint);
    }

    public async Task<HttpResponseMessage?> GetRecording(string endpoint)
    {
        return await _httpClient.GetAsync(endpoint);
    }

    public async Task<HttpResponseMessage?> DeleteRecording(string endpoint)
    {
        return await _httpClient.DeleteAsync(endpoint);
    }
}