
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using MobileMonitoringBackend.BusinessLogical.Utils.Exceptions.User;
using MobileMonitoringBackend.Contracts.DTOs.Parameters.User;
using MobileMonitoringBackend.Contracts.DTOs.Responses.User;

namespace MobileMonitoringBackendTests.Utils;

public static class HttpClientExtender
{
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
        string endpoint, object? bodyObject=null, string? token=null, string firstPutJsonParameterxD="id")
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
            case RequestType.Put:
            {
                Dictionary<string, string> parameters = new();
                HttpContent encodedContent;
                parameters = new Dictionary<string, string> { { firstPutJsonParameterxD, bodyObject.ToString() } };
                encodedContent = new FormUrlEncodedContent(parameters);
                responseMessage = await httpClient.PutAsync(endpoint, encodedContent);
                break;
            }
        }
        return responseMessage.StatusCode;
    }
    
    public static async Task<string> GetUserToken(this HttpClient httpClient, 
        string userLogin="UserLogin", string userPassword="UserPassword")
    {
        try
        {
            var userToken = (await httpClient.SendRequest<SignUpUserResponse>(RequestType.Post, "user", new SignUpUserParameter()
            {
                Login = userLogin,
                Password = userPassword
            })).AccessToken;
            if (userToken is not null)
            {
                return userToken;
            }
            throw new UserAlreadyExistsException(userLogin);
        }
        catch
        {
            var userToken = await httpClient.SendRequest<SignUpUserResponse>(RequestType.Post, "user/login", new SignUpUserParameter()
            {
                Login = "UserLogin",
                Password = "UserPassword"
            });
            return userToken.AccessToken;
        }
    }
}

public enum RequestType
{
    Post, Get, Put, Delete
}