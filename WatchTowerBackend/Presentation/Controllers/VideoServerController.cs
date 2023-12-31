using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WatchTowerBackend.BusinessLogical.Authentication;
using WatchTowerBackend.BusinessLogical.Repositories.CameraRepository;
using WatchTowerBackend.BusinessLogical.Repositories.RecordingRepository;
using WatchTowerBackend.BusinessLogical.Repositories.RoomRepository;
using WatchTowerBackend.BusinessLogical.Repositories.VideoServerRepository;
using WatchTowerBackend.BusinessLogical.Services;
using WatchTowerBackend.Contracts.DTOs.Parameters.VideoServer;
using WatchTowerBackend.Contracts.DTOs.Responses.VideoServer;
using WatchTowerBackend.Domain.Models;
using Constants = WatchTowerBackend.BusinessLogical.Utils.Constants;

namespace WatchTowerBackend.Presentation.Controllers;

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
    public StreamUrlResponse getStreamUrl(StreamUrlParameter parameter)
    {
        var camera = _cameraRepository.GetCameraById(parameter.CameraId);
        if (camera is not null)
        {
            var streamUrl = Constants.StreamBaseUrl;
            streamUrl += camera.CameraToken;
            return new()
            {
                StreamUrl = streamUrl
            };
        }
        throw new Exception("Probably camera does not exist");
    }

    [Authorize(AuthenticationSchemes = "ApiAuthenticationScheme")]
    [HttpPut("record/start")]
    public async Task<IActionResult> StartRecording(StartRecordingParameter parameter)
    {
        var camera = _cameraRepository.GetCameraById(parameter.CameraId);
        var userLogin = Request.GetUserLoginFromToken();
        if (camera is not null && userLogin == camera.Room.OwnerLogin)
        {
            var response = await _videoServerRepository.StartRecording(Constants.StartRecordingEndpoint(camera.CameraToken));
            if (response.StatusCode == HttpStatusCode.OK)
            {
                _recordingCamerasCache.Add(parameter.CameraId);
                return Ok($"Started recording for camera {parameter.CameraId}");
            }
            return BadRequest("Could not start recording");
        }
        return Unauthorized("Authentication failed - camera does not exist or you are not an owner.");
    }

    [Authorize(AuthenticationSchemes = "ApiAuthenticationScheme")]
    [HttpPut("record/stop")]
    public async Task<IActionResult> StopRecording(StopRecordingParameter parameter)
    {
        var camera = _cameraRepository.GetCameraById(parameter.CameraId);
        var userLogin = Request.GetUserLoginFromToken();
        if (camera is not null && userLogin == camera.Room.OwnerLogin)
        {
            var response = await _videoServerRepository.StopRecording(Constants.StopRecordingEndpoint(camera.CameraToken));
            if (response.StatusCode == HttpStatusCode.OK)
            {
                _recordingCamerasCache.Remove(parameter.CameraId);
                var videoPath = (await response.Content.ReadAsStringAsync()).Trim('/');
                var fileResponseMessage = await _videoServerRepository.GetRecording(videoPath);
                var file = await fileResponseMessage.Content.ReadAsStreamAsync();
                string fileName = CreateFileName(camera.RoomName, camera.CameraName);
                var videoUrl = CreateRecordingUrl(videoPath);
                var recording = _recordingRepository.AddRecording(fileName, videoUrl, camera.Room);
                if (recording is not null)
                {
                    return File(file, "application/force-download", fileName);
                }
                return BadRequest("Could not save a recording");
            }
            return BadRequest("Could not stop recording or download a file");
        }
        return Unauthorized("Authentication failed - camera does not exist or you are not an owner.");
    }

    [Authorize(AuthenticationSchemes = "ApiAuthenticationScheme")]
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
        var recording = _recordingRepository.GetRecording(recordingName);
        if (recording is not null)
        {
            var room = _roomRepository.GetRoomByName(recording.RoomName);
            if (AuthorizeRoomSpectator(room))
            {
                var fileResponseMessage = await _videoServerRepository.GetRecording(TrimRecordingUrl(recording.Url));
                var file = await fileResponseMessage.Content.ReadAsStreamAsync();
                if (recording is not null)
                {
                    return File(file, "application/force-download", recordingName);
                }
                return BadRequest("Could not save a recording");
            }
        }

        throw new Exception("Could not download recordings");
    }

    [Authorize(AuthenticationSchemes = "ApiAuthenticationScheme")]
    [HttpDelete("record/{recordingName}")]
    public async Task<IActionResult> DeleteRecording(string recordingName)
    {
        var recording = _recordingRepository.GetRecording(recordingName);
        var userLogin = Request.GetUserLoginFromToken();
        if (recording is not null && userLogin==recording.Room.OwnerLogin)
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
        if (recording is not null)
        {
            return Unauthorized("You are not an owner of this video");
        }
        return BadRequest("Such a recording does not exist");
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