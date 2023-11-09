using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WatchTowerAPI.BusinessLogical.Repositories.CameraRepository;
using WatchTowerAPI.BusinessLogical.Repositories.RoomRepository;
using WatchTowerAPI.Contracts.DTOs.Parameters;
using WatchTowerAPI.Contracts.DTOs.Parameters.Camera;
using WatchTowerAPI.Contracts.DTOs.Parameters.Room;
using WatchTowerAPI.Contracts.DTOs.Responses;
using WatchTowerAPI.Contracts.DTOs.Responses.Camera;
using WatchTowerAPI.Contracts.DTOs.Responses.Room;
using WatchTowerAPI.Domain.Models;

namespace WatchTowerAPI.Presentation.Controllers;

[ApiController]
[Route("/[controller]")]
public class cameraController : ControllerBase
{
    private readonly ICameraRepository _cameraRepository;
    private readonly IRoomRepository _roomRepository;

    public cameraController(ICameraRepository cameraRepository,
        IRoomRepository roomRepository)
    {
        _cameraRepository = cameraRepository;
        _roomRepository = roomRepository;
    }

    [HttpPost]
    public PostCameraResponse PostCamera(PostCameraParameter parameter)
    {
        var roomParamter = _roomRepository.GetRoomByName(parameter.RoomName);
        if (_roomRepository.CheckRoomAndPassword(parameter.RoomName, parameter.Password))
        {
            if (_cameraRepository.CreateCameraWithRoom(roomParamter) is not null)
            {
                return new PostCameraResponse()
                {
                    CameraToken = "token",
                    CameraUrl = "http://token"
                };
            }
        }
        throw new Exception("Can not add camera with room");
    }
    
    [Authorize]
    [HttpDelete("{id}")]
    public IActionResult DeleteCamera(Guid id)
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpPut("{id}")]
    public IActionResult AcceptCamera(Guid id)
    {
        throw new NotImplementedException();
    }
    
    /*[HttpPost("JoinRoom")]
    public bool JoinRoom(JoinRoomParameter parameter)
    {
        return _cameraRepository.AssignNewRoom(
            parameter.CameraId,
            _roomRepository.GetRoomById(parameter.RoomId));
    }

    [HttpGet("example")]
    public string Example()
    {
        return "example";
    }*/

}