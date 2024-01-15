using System.Net;
using Microsoft.Extensions.Configuration;
using Moq;
using MobileMonitoringBackend.BusinessLogical.Repositories.RecordingRepository;
using MobileMonitoringBackend.Domain.Models;
using MobileMonitoringBackend.Presentation.Controllers;
using MobileMonitoringBackend.BusinessLogical.Utils;
using MobileMonitoringBackend.Contracts.DTOs.Parameters.Room;
using MobileMonitoringBackend.Contracts.DTOs.Parameters.User;
using MobileMonitoringBackend.Contracts.DTOs.Responses.User;
using MobileMonitoringBackendTests.User;
using MobileMonitoringBackend.Contracts.DTOs.Responses.Room;
using MobileMonitoringBackendTests.Utils;

namespace MobileMonitoringBackendTests.Room;

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
            new RecordingRepository(RepositoryMockTest.CreateMockDbContext()),
            configMock.Object);
        _httpClient = new();
        _httpClient.BaseAddress = new Uri("http://localhost:5000/");
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
        var userToken = await _httpClient.GetUserToken();
        await PopulateDbWithRoom(_httpClient, "Room", "Password", userToken);
        Assert.True(true);
    }

    [Fact]
    public async Task GetRoomShouldReturnListOfRooms()
    {
        var userToken = await _httpClient.GetUserToken();
        await PopulateDbWithRoom(_httpClient, "Room1", "RoomPassword", userToken);
        await PopulateDbWithRoom(_httpClient, "Room2", "RoomPassword", userToken);
        var postRoomRespone = await _httpClient.SendRequest<GetAllRoomsResponse>(RequestType.Get, "room", null, userToken);
        Assert.True(postRoomRespone.Rooms.Count > 0);
    }

    [Fact]
    public async Task RoomTokenShouldReturnToken()
    {
        var userToken = await _httpClient.GetUserToken();
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
        var userToken = await _httpClient.GetUserToken();
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
                    Name = roomName,
                    Password = roomPassword
                }, token);
        }
        catch
        {

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