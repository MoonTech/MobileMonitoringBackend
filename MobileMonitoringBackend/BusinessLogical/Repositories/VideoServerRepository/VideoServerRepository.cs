using MobileMonitoringBackend.BusinessLogical.Utils.Exceptions.VideoServer;

namespace MobileMonitoringBackend.BusinessLogical.Repositories.VideoServerRepository;

public class VideoServerRepository : IVideoServerRepository
{
    private readonly HttpClient _httpClient;

    public VideoServerRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<HttpResponseMessage> StartRecording(string endpoint)
    {
        var result = await _httpClient.GetAsync(endpoint);
        return await ReturnOrThrowError(result);
    }

    public async Task<HttpResponseMessage> StopRecording(string endpoint)
    {
        var result = await _httpClient.GetAsync(endpoint);
        return await ReturnOrThrowError(result);
    }

    public async Task<HttpResponseMessage> GetRecording(string endpoint)
    {
        var result = await _httpClient.GetAsync(endpoint);
        return await ReturnOrThrowError(result);
    }

    public async Task<HttpResponseMessage> DeleteRecording(string endpoint)
    {
        var result = await _httpClient.DeleteAsync(endpoint);
        return await ReturnOrThrowError(result);
    }

    private async Task<HttpResponseMessage> ReturnOrThrowError(HttpResponseMessage result)
    {
        if (result.IsSuccessStatusCode)
        {
            return result;
        }
        var statusCode = (int) result.StatusCode;
        var message = await result.Content.ReadAsStringAsync();
        throw new VideoServerCommunicationException(statusCode, message);
    }
}