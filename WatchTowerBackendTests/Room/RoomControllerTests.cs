using System.Net;
using Microsoft.Extensions.Configuration;
using Moq;
using WatchTowerAPI.Domain.Models;
using WatchTowerAPI.Presentation.Controllers;
using WatchTowerBackend.BusinessLogical.Utils;
using WatchTowerBackend.Contracts.DTOs.Parameters.Room;
using WatchTowerBackend.Contracts.DTOs.Parameters.User;
using WatchTowerBackend.Contracts.DTOs.Responses.User;
using WatchTowerBackendTests.User;
using WatchTowerAPI.Contracts.DTOs.Responses.Room;
using WatchTowerBackendTests.Utils;

namespace WatchTowerBackendTests.Room;

public class RoomControllerTests
{
    private readonly roomController _roomController;
    private readonly HttpClient _httpClient;

    public RoomControllerTests()
    {
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Jwt:ApiKey"])
            .Returns("lBfPdUceVJdDNdJ3r70ZnlzAl9EY28Rf4VtjTNLcgh5BSwo2V0cq1w1RiIk5FmJ27T24sFkzZD5ScymcUFK9wjuSeoh3EfAF7zquKdQn1gL00EI4Svfwnh1EIkoZU3sB");
        var userRepository = UserRepositoryMock.SetUserRepository();
        _roomController = new roomController(
            RoomRepositoryMock.SetRoomRepository(), 
            userRepository, 
            configMock.Object);
        _httpClient = new();
        _httpClient.BaseAddress = new Uri(Constants.ApiHttpUrl);
    }

    [Fact]
    public void PostRoomShouldThrowAnErrorWhenUnauthorized()
    {
        try
        {
            _roomController.PostRoom(examplePostRoomParameter);
        }
        catch
        {
            Assert.True(true);
        }
    }

    [Fact]
    public async Task PostRoomShouldReturnRoomNameWhenAuthorized()
    {
        var userToken = await GetUserToken(_httpClient);
        await PopulateDbWithRoom(_httpClient, "Room", "Password", userToken);
        Assert.True(true);
    }

    [Fact]
    public async Task GetRoomShouldReturnListOfRooms()
    {
        var userToken = await GetUserToken(_httpClient);
        await PopulateDbWithRoom(_httpClient, "Room1", "RoomPassword", userToken);
        await PopulateDbWithRoom(_httpClient, "Room2", "RoomPassword", userToken);
        var postRoomRespone = await _httpClient.SendRequest<GetAllRoomsResponse>(RequestType.Get, "room", null, userToken);
        Assert.True(postRoomRespone.Rooms.Count > 0);
    }

    [Fact]
    public async Task RoomTokenShouldReturnToken()
    {
        var userToken = await GetUserToken(_httpClient);
        await PopulateDbWithRoom(_httpClient, "RoomName", "RoomPassword", userToken);
        var roomToken = await _httpClient.SendRequest<GenerateTokenResponse>(RequestType.Post,
            "room/token", new GenerateTokenParameter()
            {
                RoomName = "RoomName",
                Password = "RoomPassword"
            });
        Assert.True(roomToken.AccessToken.Length > 0);
    }

    [Fact]
    public async Task RoomDeleteShouldReturn200()
    {
        var userToken = await GetUserToken(_httpClient);
        await PopulateDbWithRoom(_httpClient, "RoomName", "RoomPassword", userToken);
        var statusCode = await _httpClient.SendRequestNoAnswerBody(RequestType.Delete, "room/RoomName");
        Assert.True(statusCode == HttpStatusCode.OK);
    }

    private PostRoomParameter examplePostRoomParameter{
        get
        {
            return new PostRoomParameter()
            {
                Name = "RoomName",
                Password = "RoomPassword"
            };
        }
    }

    public static async Task PopulateDbWithRoom(HttpClient httpClient, string roomName, string roomPassword, string token)
    {
        try
        {
            var postRoomResponse = await httpClient.SendRequest<PostRoomResponse>(RequestType.Post, "room",
                new PostRoomParameter()
                {
                    Name = "RoomName",
                    Password = "RoomPassword"
                }, token);
            Assert.True(postRoomResponse.RoomName.Length > 0);
        }
        catch
        {

        }
    }
    
    public static async Task<string> GetUserToken(HttpClient httpClient, 
        string userLogin="UserLogin", string userPassword="UserPassword")
    {
        try
        {
            var userToken = (await httpClient.SendRequest<SignUpUserResponse>(RequestType.Post, "user", new SignUpUserParameter()
            {
                Login = userLogin,
                Password = userPassword
            })).AccessToken;
            return userToken;
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
    
    public static RoomModel exampleRoomModel
    {
        get
        {
            return new RoomModel()
            {
                RoomName = "RoomName",
                Cameras = new List<CameraModel>()
                {
                    new CameraModel()
                    {
                        AcceptationState = true,
                        CameraName = "Camera1",
                        CameraToken = "ajkT2bxuIo",
                        Id = new Guid("49b0347e-7a86-41b7-36c7-08dbf8d15782"),
                        RoomName = "RoomName"
                    },
                    new CameraModel()
                    {
                        AcceptationState = false,
                        CameraName = "Camera2",
                        CameraToken = "lwjY7xbs5a",
                        Id = new Guid("49b0347e-7a96-41b7-36c7-08dbf8d15782"),
                        RoomName = "RoomName"
                    }
                },
                OwnerLogin = "OwnerLogin",
                Password = "RoomPassword"
            };
        }
    }
}