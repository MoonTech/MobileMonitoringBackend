using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SkiaSharp;
using SkiaSharp.QrCode.Image;
using MobileMonitoringBackend.BusinessLogical.Authentication;
using MobileMonitoringBackend.BusinessLogical.Converters;
using MobileMonitoringBackend.BusinessLogical.Repositories.RecordingRepository;
using MobileMonitoringBackend.BusinessLogical.Repositories.RoomRepository;
using MobileMonitoringBackend.BusinessLogical.Repositories.UserRepository;
using MobileMonitoringBackend.BusinessLogical.Utils;
using MobileMonitoringBackend.BusinessLogical.Utils.Exceptions;
using MobileMonitoringBackend.BusinessLogical.Utils.Exceptions.Room;
using MobileMonitoringBackend.Contracts.DTOs.Parameters.Room;
using MobileMonitoringBackend.Contracts.DTOs.Responses.Room;
using MobileMonitoringBackend.Domain.Models;

namespace MobileMonitoringBackend.Presentation.Controllers;

[ApiController]
[Route("/room")]
public class roomController : ControllerBase
{
    private readonly IRoomRepository _roomRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRecordingRepository _recordingRepository;
    private readonly IConfiguration _config;
    private readonly int _tokenValidHours = 1;
    private readonly int _refreshTokenValidHours = 1;

    public roomController(IRoomRepository roomRepository,
        IUserRepository userRepository,
        IRecordingRepository recordingRepository,
        IConfiguration config)
    {
        _roomRepository = roomRepository;
        _userRepository = userRepository;
        _recordingRepository = recordingRepository;
        _config = config;
    }

    [Authorize(AuthenticationSchemes = Constants.ApiAuthScheme)]
    [HttpPost]
    public ActionResult<PostRoomResponse> PostRoom(PostRoomParameter parameter)
    {
        try
        {
            var userLogin = Request.GetUserLoginFromToken();
            var user = _userRepository.GetUser(userLogin);
            _roomRepository.CreateRoom(parameter.Name, parameter.Password, user);
            return Ok(new PostRoomResponse()
            {
                RoomName = parameter.Name
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

    [Authorize]
    [HttpPost("watch")]
    public ActionResult<WatchResponse> Watch(WatchParameter parameter)
    {
        try
        {
            var room = _roomRepository.GetRoomByName(parameter.RoomName);
            if (AuthorizeRoomSpectator(room))
            {
                var response = new WatchResponse()
                {
                    RoomName = room.RoomName
                };
                foreach (var camera in room.Cameras)
                {
                    if (camera.AcceptationState)
                    {
                        response.ConnectedCameras.Add(camera);
                    }
                }
                return Ok(response);
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
    [HttpDelete("{roomName}")]
    public IActionResult DeleteRoom(string roomName)
    {
        try
        {
            var userLogin = Request.GetUserLoginFromToken();
            var roomToDelete = _roomRepository.GetRoomByName(roomName);
            if (roomToDelete.OwnerLogin == userLogin)
            {
                if (_roomRepository.DeleteRoom(roomToDelete))
                {
                    return Ok("Room has been succesfully removed");
                }
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
    [HttpGet]
    public ActionResult<GetAllRoomsResponse> GetAllRooms()
    {
        try
        {
            var login = Request.GetUserLoginFromToken();
            var user = _userRepository.GetUser(login);
            return Ok(new GetAllRoomsResponse
            {
                Rooms = WithoutPasswordConverter.RoomCollectionConverter(user.Rooms)
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
    [HttpGet("{roomName}")]
    public ActionResult<GetPendingCamerasResponse> GetPendingCameras(string roomName)
    {
        try
        {
            var login = Request.GetUserLoginFromToken();
            var room = _roomRepository.GetRoomByName(roomName);
            if (room.OwnerLogin == login)
            {
                var response = new GetPendingCamerasResponse();
                foreach (var camera in room.Cameras)
                {
                    if (camera.AcceptationState == false)
                    {
                        response.PendingCameras.Add(camera);
                    }
                }
                return Ok(response);
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

    [Authorize]
    [HttpGet("recordings/{roomName}")]
    public ActionResult<GetRecordingsResponse> GetRecordings(string roomName)
    {
        try
        {
            var room = _roomRepository.GetRoomByName(roomName);
            if (AuthorizeRoomSpectator(room))
            {
                var recordings = WithoutPasswordConverter.RecordingCollectionConverter(
                    _recordingRepository.GetRoomRecordings(room));
                return Ok(new GetRecordingsResponse
                {
                    Recordings = recordings
                });
            }
            throw new RoomAccessDeniedException(roomName);
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
    [HttpGet("qrCode/{roomName}")]
    public IActionResult GenerateQrCode(string roomName)
    {
        try
        {
            var room = _roomRepository.GetRoomByName(roomName);
            var userLogin = Request.GetUserLoginFromToken();
            if (userLogin == room.OwnerLogin)
            {
                var qrBodyObject = new
                {
                    roomName = room.RoomName,
                    token = GenerateRoomToken(room),
                    refreshToken = GenerateRefreshToken(room)
                };
                string qrBodyString = JsonSerializer.Serialize(qrBodyObject);
                var qrCodeStreamName = room.RoomName + "_qr.png";
                return GenerateQrCodePngResponse(qrBodyString, qrCodeStreamName);
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
    
    

    [AllowAnonymous]
    [HttpPost("token")]
    public ActionResult<GenerateTokenResponse> GenerateToken(GenerateTokenParameter parameter)
    {
        try
        {
            var room = _roomRepository.GetRoom(parameter.RoomName, parameter.Password);
            var token = GenerateRoomToken(room);
            var refreshToken = GenerateRefreshToken(room);
            SetRefreshToken(refreshToken, room.RoomName);
            return Ok(new GenerateTokenResponse
            {
                AccessToken = token
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

    [AllowAnonymous]
    [HttpPost("refreshToken/{roomName}")]
    public ActionResult<RefreshRoomTokenResponse> RefreshToken(string roomName)
    {
        try
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var roomNameFromCookie = JwtSecurityTokenExtension.GetClaim(refreshToken, "RoomName");
            if (Request.GetRoomNameFromToken() != roomNameFromCookie || roomNameFromCookie != roomName)
            {
                throw new RoomAccessRefreshException();
            }

            var room = _roomRepository.GetRoomByName(roomName);
            if (ValidateRefreshToken(refreshToken))
            {
                var newRefreshToken = GenerateRefreshToken(room);
                SetRefreshToken(newRefreshToken, roomName);
                string token = GenerateRoomToken(room);
                var result = new RefreshRoomTokenResponse()
                {
                    accessToken = token
                };
                return Ok(result);
            }
            throw new MobileMonitoringException(StatusCodes.Status400BadRequest,
                ErrorCodes.InvalidRefreshToken,
                "Invalid refresh token");
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
    
    // Additional Methods
    private string GenerateRoomToken(RoomModel room)
    {
        return JwtSecurityTokenExtension.GenerateToken(
            _config,
            "Jwt:RoomKey",
            _tokenValidHours,
            new[]
            {
                new Claim("RoomName", room.RoomName),
                new Claim("type", "RoomAuth")
            });
    }

    private string GenerateRoomRefreshToken(RoomModel room)
    {
        return JwtSecurityTokenExtension.GenerateToken(
            _config,
            "Jwt:RoomRefreshKey",
            _refreshTokenValidHours,
            new[]
            {
                new Claim("RoomName", room.RoomName),
                new Claim("type", "RoomRefreshToken")
            });
    }

    private bool AuthorizeRoomSpectator(RoomModel room)
    {
        var userLogin = Request.GetUserLoginFromToken();
        var roomName = Request.GetRoomNameFromToken();
        return (room is not null && userLogin is not null && userLogin == room.OwnerLogin)
                || (roomName is not null);
    }

    private byte[] GenerateQRbyteArray(string qrBody)
    {
        byte[] buffer;
        using var memoryStream = new MemoryStream();
        var qrCode = new QrCode(qrBody, new Vector2Slim(256, 256), SKEncodedImageFormat.Png);
        qrCode.GenerateImage(memoryStream);
        memoryStream.Position = 0;
        try
        {
            int length = (int)memoryStream.Length;
            buffer = new byte[length];
            int count;
            int sum = 0;
            while ((count = memoryStream.Read(buffer, sum, length - sum)) > 0)
                sum += count;
        }
        finally
        {
            memoryStream.Close();
        }
        return buffer;
    }

    private IActionResult GenerateQrCodePngResponse(string qrContent, string fileName)
    {
        var qrByteArray = GenerateQRbyteArray(qrContent);
        return File(qrByteArray, "application/force-download", fileName);
    }
    
    private RefreshToken GenerateRefreshToken(RoomModel room)
    {
        var refreshToken = new RefreshToken
        {
            Token = GenerateRoomRefreshToken(room),
            Expires = DateTime.Now.AddHours(_refreshTokenValidHours),
            Created = DateTime.Now
        };

        return refreshToken;
    }

    private void SetRefreshToken(RefreshToken newRefreshToken, string roomName)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = newRefreshToken.Expires,
            Path = $"/room/refreshToken/{roomName}"
        };
        Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);
    }

    private bool ValidateRefreshToken(string refreshToken)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters tokenValidationParameters = Constants.TokenValidationParameters(
                _config, "Jwt:RoomRefreshKey");
            tokenHandler.ValidateToken(refreshToken, tokenValidationParameters, out _);
            return true;
        }
        catch (SecurityTokenValidationException)
        {
            return false;
        }
    }
}