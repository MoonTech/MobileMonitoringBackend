using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileMonitoringBackend.BusinessLogical.Authentication;
using MobileMonitoringBackend.BusinessLogical.Repositories.CameraRepository;
using MobileMonitoringBackend.BusinessLogical.Repositories.RoomRepository;
using MobileMonitoringBackend.BusinessLogical.Utils;
using MobileMonitoringBackend.BusinessLogical.Utils.Exceptions;
using MobileMonitoringBackend.BusinessLogical.Utils.Exceptions.Room;
using MobileMonitoringBackend.Contracts.DTOs.Parameters.Camera;
using MobileMonitoringBackend.Contracts.DTOs.Responses.Camera;
using MobileMonitoringBackend.Domain.Models;

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
    public ActionResult<PostCameraResponse> PostCamera(PostCameraParameter parameter)
    {
        try
        {
            RoomModel roomParameter = _roomRepository.GetRoomByName(parameter.RoomName);
            string? userLogin;
            try
            {
                userLogin = Request.GetUserLoginFromToken();
            }
            catch (Exception ex)
            {
                userLogin = null;
            }
            if (userLogin == roomParameter.OwnerLogin
                || _roomRepository.CheckRoomAndPassword(parameter.RoomName, parameter.Password))
            {
                var newCamera = _cameraRepository.CreateCameraWithRoom(
                    parameter.CameraName, roomParameter);
                if (userLogin == roomParameter.OwnerLogin)
                {
                    _cameraRepository.AcceptCamera(newCamera);
                }
                return Ok(new PostCameraResponse()
                {
                    Id = newCamera.Id
                });
            }
            throw new RoomPasswordWrongException(roomParameter.RoomName);
        }        
        catch (MobileMonitoringException ex)
        {
            return StatusCode(ex.HttpCode, new MobileMonitoringExceptionJSON(ex));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message); 
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteCamera(Guid id)
    {
        try
        {
            var cameraToDelete = _cameraRepository.GetCameraById(id);
            if (_cameraRepository.DeleteCamera((cameraToDelete)))
            {
                return Ok("Camera has been deleted");
            }
            throw new CouldNotSaveChangesException();
        }
        catch (MobileMonitoringException ex)
        {
            return StatusCode(ex.HttpCode, new MobileMonitoringExceptionJSON(ex));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize(AuthenticationSchemes = Constants.ApiAuthScheme)]
    [HttpPut("{id}")]
    public IActionResult AcceptCamera(Guid id)
    {
        try
        {
            var userLogin = Request.GetUserLoginFromToken();
            var cameraToAccept = _cameraRepository.GetCameraById(id);
            if (userLogin == cameraToAccept.Room!.OwnerLogin)
            {
                if (_cameraRepository.AcceptCamera(cameraToAccept))
                {
                    return Ok("Camera Accepted");
                }
            }
            throw new CouldNotSaveChangesException();
        }
        catch (MobileMonitoringException ex)
        {
            return StatusCode(ex.HttpCode, new MobileMonitoringExceptionJSON(ex));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message); 
        }
    }
}