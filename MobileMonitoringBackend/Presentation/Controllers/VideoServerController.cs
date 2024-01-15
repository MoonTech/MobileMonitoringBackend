using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileMonitoringBackend.BusinessLogical.Authentication;
using MobileMonitoringBackend.BusinessLogical.Repositories.CameraRepository;
using MobileMonitoringBackend.BusinessLogical.Repositories.RecordingRepository;
using MobileMonitoringBackend.BusinessLogical.Repositories.RoomRepository;
using MobileMonitoringBackend.BusinessLogical.Repositories.VideoServerRepository;
using MobileMonitoringBackend.BusinessLogical.Services;
using MobileMonitoringBackend.BusinessLogical.Utils.Exceptions;
using MobileMonitoringBackend.BusinessLogical.Utils.Exceptions.Recording;
using MobileMonitoringBackend.BusinessLogical.Utils.Exceptions.Room;
using MobileMonitoringBackend.Contracts.DTOs.Parameters.VideoServer;
using MobileMonitoringBackend.Contracts.DTOs.Responses.VideoServer;
using MobileMonitoringBackend.Domain.Models;
using Constants = MobileMonitoringBackend.BusinessLogical.Utils.Constants;

namespace MobileMonitoringBackend.Presentation.Controllers;

[ApiController]
[Route("/[controller]")]
public class videoServerController : ControllerBase
{
    private readonly ICameraRepository _cameraRepository;
    private readonly IRecordingRepository _recordingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly RecordingCamerasCache _recordingCamerasCache;
    private readonly IVideoServerRepository _videoServerRepository;

    public videoServerController(
        ICameraRepository cameraRepository,
        IRecordingRepository recordingRepository,
        IRoomRepository roomRepository,
        RecordingCamerasCache recordingCamerasCache,
        IVideoServerRepository videoServerRepository)
    {
        _cameraRepository = cameraRepository;
        _recordingRepository = recordingRepository;
        _roomRepository = roomRepository;
        _recordingCamerasCache = recordingCamerasCache;
        _videoServerRepository = videoServerRepository;
    }
    
    [AllowAnonymous]
    [HttpPost("streamUrl")]
    public ActionResult<StreamUrlResponse> GetStreamUrl(StreamUrlParameter parameter)
    {
        try
        {
            var camera = _cameraRepository.GetCameraById(parameter.CameraId);
            var streamUrl = Constants.StreamBaseUrl;
            streamUrl += camera.CameraToken;
            return Ok(new StreamUrlResponse
            {
                StreamUrl = streamUrl
            });
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
    [HttpPut("record/start")]
    public async Task<IActionResult> StartRecording(StartRecordingParameter parameter)
    {
        try
        {
            var camera = _cameraRepository.GetCameraById(parameter.CameraId);
            var userLogin = Request.GetUserLoginFromToken();
            if (userLogin == camera.Room!.OwnerLogin)
            {
                var response =
                    await _videoServerRepository.StartRecording(Constants.StartRecordingEndpoint(camera.CameraToken));
                if (response.StatusCode == HttpStatusCode.OK 
                    && !(_recordingCamerasCache.Contains(parameter.CameraId)))
                {
                    _recordingCamerasCache.Add(parameter.CameraId);
                    return Ok($"Started recording for camera {parameter.CameraId}");
                }
                throw new RecordingCamerasCacheException();
            }
            throw new RoomWrongOwnerException();
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
    [HttpPut("record/stop")]
    public async Task<IActionResult> StopRecording(StopRecordingParameter parameter)
    {
        try
        {
            var camera = _cameraRepository.GetCameraById(parameter.CameraId);
            var userLogin = Request.GetUserLoginFromToken();
            if (userLogin == camera.Room!.OwnerLogin)
            {
                var response =
                    await _videoServerRepository.StopRecording(Constants.StopRecordingEndpoint(camera.CameraToken));
                if (response.StatusCode == HttpStatusCode.OK 
                    && _recordingCamerasCache.Contains(parameter.CameraId))
                {
                    _recordingCamerasCache.Remove(parameter.CameraId);
                    var videoPath = (await response.Content.ReadAsStringAsync()).Trim('/');
                    var fileResponseMessage = await _videoServerRepository.GetRecording(videoPath);
                    var file = await fileResponseMessage.Content.ReadAsStreamAsync();
                    string fileName = CreateFileName(camera.RoomName, camera.CameraName);
                    var videoUrl = CreateRecordingUrl(videoPath);
                    _recordingRepository.AddRecording(fileName, videoUrl, camera.Room);
                    return File(file, "application/force-download", fileName);
                }

                throw new RecordingCamerasCacheException();
            }
            throw new RoomWrongOwnerException();
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
    [HttpGet("record/check")]
    public IActionResult CheckRecording([Required]Guid id)
    {
        // returns true if camera is not transmitting 
        return _recordingCamerasCache.Contains(id) ? Ok(false) : Ok(true);
    }

    [Authorize]
    [HttpGet("record/{recordingName}")]
    public async Task<IActionResult> DownloadRecording(string recordingName)
    {
        try
        {
            var recording = _recordingRepository.GetRecording(recordingName);
            var room = _roomRepository.GetRoomByName(recording.RoomName);
            if (AuthorizeRoomSpectator(room))
            {
                var fileResponseMessage = await _videoServerRepository.GetRecording(TrimRecordingUrl(recording.Url));
                var file = await fileResponseMessage.Content.ReadAsStreamAsync();
                return File(file, "application/force-download", recordingName);
            }
            throw new RoomAccessDeniedException(room.RoomName);
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
    [HttpDelete("record/{recordingName}")]
    public async Task<IActionResult> DeleteRecording(string recordingName)
    {
        try
        {
            var recording = _recordingRepository.GetRecording(recordingName);
            var userLogin = Request.GetUserLoginFromToken();
            if (userLogin == recording.Room!.OwnerLogin)
            {
                var response = await _videoServerRepository.DeleteRecording(TrimRecordingUrl(recording.Url));
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    if (_recordingRepository.DeleteRecording(recording))
                    {
                        return Ok("Recording deleted Succesfully");
                    }
                }

                return BadRequest("Deleting recording failed");
            }
            throw new RoomWrongOwnerException();
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
    
    private bool AuthorizeRoomSpectator(RoomModel room)
    {
        var userLogin = Request.GetUserLoginFromToken();
        var roomName = Request.GetRoomNameFromToken();
        return (room is not null && userLogin is not null && userLogin == room.OwnerLogin)
               || (roomName is not null);
    }

    private string CreateFileName(string roomName, string cameraName)
    {
        return roomName 
               + "-" 
               + cameraName 
               + "-"
               + DateTime.Now.ToString("dd-MM-yyyy-HH_mm_ss")
               + ".flv";
    }

    private string CreateRecordingUrl(string fileName)
    {
        return Constants.RecordBaseUrl + fileName;
    }

    private string TrimRecordingUrl(string recordingUrl)
    {
        var baseAddress = Constants.RecordBaseUrl;
        int index = recordingUrl.IndexOf(baseAddress);
        string cleanPath = (index < 0)
            ? recordingUrl
            : recordingUrl.Remove(index, baseAddress.Length);
        return cleanPath;
    }
}