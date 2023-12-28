using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WatchTowerBackend.BusinessLogical.Authentication;
using WatchTowerBackend.BusinessLogical.Repositories.CameraRepository;
using WatchTowerBackend.BusinessLogical.Repositories.RoomRepository;
using WatchTowerBackend.Contracts.DTOs.Parameters.Camera;
using WatchTowerBackend.Contracts.DTOs.Responses.Camera;

namespace WatchTowerBackend.Presentation.Controllers;

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

    [Authorize(AuthenticationSchemes = "ApiAuthenticationScheme")]
    [HttpPost]
    public PostCameraResponse PostCamera(PostCameraParameter parameter)
    {
        var roomParameter = _roomRepository.GetRoomByName(parameter.RoomName);
        var userLogin = Request.GetUserLoginFromToken();
        if (roomParameter is null)
        {
            throw new Exception("Such room does not exist");
        }
        if (userLogin == roomParameter.OwnerLogin
            || _roomRepository.CheckRoomAndPassword(parameter.RoomName, parameter.Password))
        {
            var newCamera = _cameraRepository.CreateCameraWithRoom(
                    parameter.CameraName, roomParameter);
            if (newCamera is not null)
            {
                if (userLogin == roomParameter.OwnerLogin)
                {
                    _cameraRepository.AcceptCamera(newCamera);
                }
                return new PostCameraResponse()
                {
                    Id = newCamera.Id
                };
            }
        }
        throw new Exception("Can not add camera with room");
    }
    
    [Authorize(AuthenticationSchemes = "ApiAuthenticationScheme")]
    [HttpDelete("{id}")]
    public IActionResult DeleteCamera(Guid id)
    {
        var cameraToDelete = _cameraRepository.GetCameraById(id);
        if (cameraToDelete is not null)
        {
            if (_cameraRepository.DeleteCamera((cameraToDelete)))
            {
                return Ok("Camera has been deleted");
            }
        }
        return BadRequest("Camera could not be deleted");
    }

    [Authorize(AuthenticationSchemes = "ApiAuthenticationScheme")]
    [HttpPut("{id}")]
    public IActionResult AcceptCamera(Guid id)
    {
        var userLogin = Request.GetUserLoginFromToken();
        var cameraToAccept = _cameraRepository.GetCameraById(id);
        if (cameraToAccept is not null 
            && userLogin == cameraToAccept.Room!.OwnerLogin)
        {
            if (_cameraRepository.AcceptCamera(cameraToAccept))
            {
                return Ok("Camera Accepted");
            }
        }
        return BadRequest("Camera Could not be accepted");
    }
}