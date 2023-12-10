using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Web.Http.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using WatchTowerAPI.BusinessLogical.Repositories.CameraRepository;
using WatchTowerAPI.BusinessLogical.Repositories.UserRepository;
using WatchTowerBackend.BusinessLogical.Authentication;
using WatchTowerBackend.BusinessLogical.Services;
using WatchTowerBackend.Contracts.DTOs.Parameters.VideoServer;
using WatchTowerBackend.Contracts.DTOs.Responses.VideoServer;
using Constants = WatchTowerBackend.BusinessLogical.Utils.Constants;

namespace WatchTowerAPI.Presentation.Controllers;

[ApiController]
[Route("/[controller]")]
public class videoServerController : ControllerBase
{
    private readonly ICameraRepository _cameraRepository;
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;
    private readonly RecordingCamerasCache _recordingCamerasCache;

    public videoServerController(ICameraRepository cameraRepository,
        IConfiguration config, RecordingCamerasCache recordingCamerasCache)
    {
        _cameraRepository = cameraRepository;
        _config = config;
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new (Constants.RecordBaseUrl);
        _recordingCamerasCache = recordingCamerasCache;
    }

    /*[AllowAnonymous]
    [HttpGet]
    public IActionResult Auth()
    {
        return Ok();
    }*/

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
            var response = await _httpClient.GetAsync(Constants.StartRecordingEndpoint(camera.CameraToken));
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
            var response = await _httpClient.GetAsync(Constants.StopRecordingEndpoint(camera.CameraToken));
            if (response.StatusCode == HttpStatusCode.OK)
            {
                _recordingCamerasCache.Remove(parameter.CameraId);
                var videoPath = await response.Content.ReadAsStringAsync();
                var fileResponseMessage = await _httpClient.GetAsync(videoPath);
                var file = await fileResponseMessage.Content.ReadAsStreamAsync();
                string fileName = parameter.CameraId + "-"
                                                     + DateTime.Now.ToString("dd-MM-yyyy-HH:mm:ss")
                                                     + ".flv";
                return File(file, "application/force-download", fileName);
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
    
}