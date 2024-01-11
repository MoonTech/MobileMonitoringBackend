using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileMonitoringBackend.BusinessLogical.Authentication;
using MobileMonitoringBackend.BusinessLogical.Repositories.CameraRepository;
using MobileMonitoringBackend.BusinessLogical.Repositories.RoomRepository;
using MobileMonitoringBackend.BusinessLogical.Utils;
using MobileMonitoringBackend.Contracts.DTOs.Parameters.Camera;
using MobileMonitoringBackend.Contracts.DTOs.Responses.Camera;

namespace MobileMonitoringBackend.Presentation.Controllers;

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

    [Authorize(AuthenticationSchemes = Constants.ApiAuthScheme)]
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