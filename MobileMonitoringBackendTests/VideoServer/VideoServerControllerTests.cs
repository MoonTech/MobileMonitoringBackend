using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using MobileMonitoringBackend.BusinessLogical.Repositories.RecordingRepository;
using MobileMonitoringBackend.BusinessLogical.Repositories.VideoServerRepository;
using MobileMonitoringBackend.Contracts.DTOs.Parameters.Camera;
using MobileMonitoringBackend.Contracts.DTOs.Responses.Camera;
using MobileMonitoringBackend.Domain.Models;
using MobileMonitoringBackend.Presentation.Controllers;
using MobileMonitoringBackend.BusinessLogical.Services;
using MobileMonitoringBackendTests.Camera;
using MobileMonitoringBackendTests.Room;
using MobileMonitoringBackendTests.Utils;
using Constants = MobileMonitoringBackend.BusinessLogical.Utils.Constants;

namespace MobileMonitoringBackendTests.VideoServer;

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
    public void StreamUrlShouldReturnOkObjectResult()
    {
        var response = _videoServerController.GetStreamUrl(new()
        {
            CameraId = _cameraModel.Id
        });
        Assert.True(response.Result is OkObjectResult);
    }

    [Fact]
    public async Task VideoChcekShouldReturn200()
    {
        var userToken = await _httpClient.GetUserToken();
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