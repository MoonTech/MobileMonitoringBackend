using Microsoft.AspNetCore.Mvc;
using WatchTowerAPI.BusinessLogical.Repositories.CameraRepository;
using WatchTowerAPI.BusinessLogical.Repositories.RoomRepository;
using WatchTowerAPI.Contracts.DTOs.Parameters;
using WatchTowerAPI.Contracts.DTOs.Parameters.Camera;
using WatchTowerAPI.Contracts.DTOs.Parameters.Room;
using WatchTowerAPI.Contracts.DTOs.Responses;
using WatchTowerAPI.Contracts.DTOs.Responses.Room;

namespace WatchTowerAPI.Presentation.Controllers;

[ApiController]
[Route("/[controller]")]
public class CameraController : ControllerBase
{
    private readonly ICameraRepository _cameraRepository;
    private readonly IRoomRepository _roomRepository;

    public CameraController(ICameraRepository cameraRepository,
        IRoomRepository roomRepository)
    {
        _cameraRepository = cameraRepository;
        _roomRepository = roomRepository;
    }
    
    [HttpPost("JoinRoom")]
    public bool JoinRoom(JoinRoomParameter parameter)
    {
        return _cameraRepository.AssignNewRoom(
            parameter.CameraId,
            _roomRepository.GetRoomById(parameter.RoomId));
    }
    
}