using System.Net;
using Microsoft.Extensions.Configuration;
using Moq;
using WatchTowerBackend.BusinessLogical.Repositories.RecordingRepository;
using WatchTowerBackend.BusinessLogical.Repositories.VideoServerRepository;
using WatchTowerBackend.Contracts.DTOs.Parameters.Camera;
using WatchTowerBackend.Contracts.DTOs.Responses.Camera;
using WatchTowerBackend.Domain.Models;
using WatchTowerBackend.Presentation.Controllers;
using WatchTowerBackend.BusinessLogical.Services;
using WatchTowerBackendTests.Camera;
using WatchTowerBackendTests.Room;
using WatchTowerBackendTests.Utils;
using Constants = WatchTowerBackend.BusinessLogical.Utils.Constants;

namespace WatchTowerBackendTests.VideoServer;

public class VideoServerControllerTests
{
    private readonly videoServerController _videoServerController;
    private readonly HttpClient _httpClient;
    private readonly RoomModel _roomModel;
    private readonly CameraModel _cameraModel;

    public VideoServerControllerTests()
    {
        var configMock = new Mock<IConfiguration>();
        var videoServerHttpClient = new HttpClient();
        videoServerHttpClient.BaseAddress = new Uri(Constants.RecordBaseUrl);
        configMock.Setup(c => c["Jwt:ApiKey"])
            .Returns("fsekljsaKLJjkjkljlj988877888hjHjjkHJKHkhgfDEfsr6gVBnbJgjJHsdnfsasjB898hghVVSSFghjjhfDgBj86BGJjjjkiufcfdssFGT64fBB");
        _videoServerController = new(CameraRepositoryMock.SetCameraRepository(ref _roomModel, ref _cameraModel),
            new RecordingRepository(RepositoryMockTest.CreateMockDbContext()), 
            RoomRepositoryMock.SetRoomRepository(), new RecordingCamerasCache(), 
            new VideoServerRepository(videoServerHttpClient));
        _httpClient = new();
        _httpClient.BaseAddress = new Uri("http://localhost:5000/");
    }

    [Fact]
    public void StreamUrlShouldReturnUrl()
    {
        var response = _videoServerController.getStreamUrl(new()
        {
            CameraId = _cameraModel.Id
        });
        Assert.True(response.StreamUrl.Length > 0);
    }

    [Fact]
    public async Task VideoChcekShouldReturn200() // TODO Change this test to return true
    {
        var userToken = await RoomControllerTests.GetUserToken(_httpClient); // TODO Move all the static methods to other place
        await RoomControllerTests.PopulateDbWithRoom(_httpClient, "Room1", "RoomPassword", userToken);
        try
        {
            await _httpClient.SendRequest<PostCameraResponse>(RequestType.Post, "camera",
                new PostCameraParameter()
                {
                    CameraName = "Camera1",
                    RoomName = "Room1",
                    Password = ""
                });
        }
        catch
        {
         
        }
        var guid = await CameraControllerTests.GetGuidOfCamera1Room1(_httpClient);
        var response = await _httpClient.SendRequestNoAnswerBody(RequestType.Get
            , $"videoServer/record/check?id={guid}"
            ,token:userToken);
        Assert.True(response == HttpStatusCode.OK);
    }
}