
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace WatchTowerBackendTests.Utils;

public static class HttpClientExtender
{
    // TODO These two functions are horrible
    public static async Task<T> SendRequest<T>(this HttpClient httpClient, RequestType requestType, 
        string endpoint, object? bodyObject=null, string? token=null)
    {
        HttpResponseMessage? responseMessage = new();

        if (token is not null)
        {
            httpClient.DefaultRequestHeaders.Authorization 
                = new AuthenticationHeaderValue("Bearer", token);
        }
        
        switch (requestType)
        {
            case RequestType.Get:
            {
                responseMessage = await httpClient.GetAsync(endpoint);
                break;
            }
            case RequestType.Post:
            {
                var body = JsonContent.Create(bodyObject);
                responseMessage = await httpClient.PostAsync(endpoint,body);
                break;
            }
        }
        var responseString = await responseMessage.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<T>(responseString, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        return result;
    }
    public static async Task<HttpStatusCode> SendRequestNoAnswerBody(this HttpClient httpClient, RequestType requestType, 
        string endpoint, object? bodyObject=null, string? token=null)
    {
        HttpResponseMessage? responseMessage = new();

        if (token is not null)
        {
            httpClient.DefaultRequestHeaders.Authorization 
                = new AuthenticationHeaderValue("Bearer", token);
        }
        
        switch (requestType)
        {
            case RequestType.Get:
            {
                responseMessage = await httpClient.GetAsync(endpoint);
                break;
            }
            case RequestType.Post:
            {
                var body = JsonContent.Create(bodyObject);
                responseMessage = await httpClient.PostAsync(endpoint,body);
                break;
            }
            case RequestType.Delete:
            {
                var body = JsonContent.Create(bodyObject);
                responseMessage = await httpClient.DeleteAsync(endpoint,new CancellationToken());
                break;
            }
        }
        return responseMessage.StatusCode;
    }
}

public enum RequestType
{
    Post, Get, Put, Delete
}