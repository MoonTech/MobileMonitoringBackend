using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using WatchTowerAPI.BusinessLogical.Repositories.CameraRepository;
using WatchTowerAPI.BusinessLogical.Repositories.UserRepository;
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

    public videoServerController(ICameraRepository cameraRepository,
        IConfiguration config)
    {
        _cameraRepository = cameraRepository;
        _config = config;
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
}