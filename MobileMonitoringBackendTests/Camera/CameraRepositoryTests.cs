using MobileMonitoringBackend.BusinessLogical.Repositories.CameraRepository;
using MobileMonitoringBackend.BusinessLogical.Repositories.RoomRepository;
using MobileMonitoringBackend.BusinessLogical.Utils.Exceptions.Camera;
using MobileMonitoringBackend.Domain.Models;
using MobileMonitoringBackendTests.Utils;

namespace MobileMonitoringBackendTests.Camera;

public class CameraRepositoryTests
{
    private readonly ICameraRepository _cameraRepositoryMock;
    private readonly RoomModel _exampleRoom;
    private readonly CameraModel _exampleCamera;

    public CameraRepositoryTests()
    {
        _cameraRepositoryMock = CameraRepositoryMock.SetCameraRepository(ref _exampleRoom, ref _exampleCamera);
    }

    [Fact]
    public void CreateCameraWithRoomShouldReturnCamera()
    {
        var camera = _cameraRepositoryMock.CreateCameraWithRoom("Camera2", _exampleRoom);
        Assert.True(camera is not null);
    }

    [Fact]
    public void GetCameraByIdShouldReturnCamera()
    {
        var result = _cameraRepositoryMock.GetCameraById(_exampleCamera.Id);
        Assert.True(result is not null);
    }

    [Fact]
    public void DeleteCameraShouldReturnTrue()
    {
        var result = _cameraRepositoryMock.DeleteCamera(_exampleCamera);
        Assert.True(result);
    }

    [Fact]
    public void AcceptCameraShouldReturnTrue()
    {
        var result = _cameraRepositoryMock.AcceptCamera(_exampleCamera);
        Assert.True(result);
        Assert.True(_exampleCamera.AcceptationState);
    }

    [Fact]
    public void AcceptNonExistingCameraShouldThrowCameraDoesNotExistException()
    {
        try
        {
            _cameraRepositoryMock.AcceptCamera(new CameraModel()
            {
                CameraName = "NonExistingCamera"
            });
        }
        catch (Exception ex)
        {
            Assert.True(ex is CameraDoesNotExistException);
        }
    }
}

public static class CameraRepositoryMock
{
    internal static ICameraRepository SetCameraRepository(ref RoomModel roomModel, ref CameraModel cameraModel)
    {
        var mockDbContext = RepositoryMockTest.CreateMockDbContext();
        var cameraRepository = new CameraRepository(mockDbContext);
        var roomRepository = new RoomRepository(mockDbContext);
        var room = roomRepository.CreateRoom("NewRoom", "NewPassword", new UserModel()
        {
            Login = "NewUserLogin",
            Password = "NewUserPassword"
        });
        roomModel = room;
        cameraModel = cameraRepository.CreateCameraWithRoom("Camera1", room);
        return cameraRepository;
    }
}