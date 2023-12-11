using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Moq;
using WatchTowerAPI.Contracts.DTOs.Parameters.Camera;
using WatchTowerAPI.Contracts.DTOs.Responses.Camera;
using WatchTowerAPI.Domain.Models;
using WatchTowerAPI.Presentation.Controllers;
using WatchTowerBackend.BusinessLogical.Services;
using WatchTowerBackend.Contracts.DTOs.Parameters.VideoServer;
using WatchTowerBackendTests.Camera;
using WatchTowerBackendTests.Room;
using WatchTowerBackendTests.User;
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
        configMock.Setup(c => c["Jwt:ApiKey"])
            .Returns("fsekljsaKLJjkjkljlj988877888hjHjjkHJKHkhgfDEfsr6gVBnbJgjJHsdnfsasjB898hghVVSSFghjjhfDgBj86BGJjjjkiufcfdssFGT64fBB");
        _videoServerController = new(CameraRepositoryMock.SetCameraRepository(ref _roomModel, ref _cameraModel),
            configMock.Object, new RecordingCamerasCache());
        _httpClient = new();
        _httpClient.BaseAddress = new Uri(Constants.ApiHttpUrl);
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