using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SkiaSharp;
using SkiaSharp.QrCode.Image;
using WatchTowerBackend.BusinessLogical.Authentication;
using WatchTowerBackend.BusinessLogical.Converters;
using WatchTowerBackend.BusinessLogical.Repositories.RecordingRepository;
using WatchTowerBackend.BusinessLogical.Repositories.RoomRepository;
using WatchTowerBackend.BusinessLogical.Repositories.UserRepository;
using WatchTowerBackend.BusinessLogical.Utils;
using WatchTowerBackend.Contracts.DTOs.Parameters.Room;
using WatchTowerBackend.Contracts.DTOs.Responses.Room;
using WatchTowerBackend.Domain.Models;

namespace WatchTowerBackend.Presentation.Controllers;

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

    [Authorize(AuthenticationSchemes = "ApiAuthenticationScheme")]
    [HttpPost]
    public PostRoomResponse PostRoom(PostRoomParameter parameter)
    {
        var userLogin = Request.GetUserLoginFromToken();
        var user = _userRepository.GetUser(userLogin);
        if (user is not null)
        {
            var newRoom = _roomRepository.CreateRoom(parameter.Name, parameter.Password, user);
            // TODO Add room to user
             if (newRoom is not null)
            {
                return new()
                {
                    RoomName = parameter.Name
                };
            }
        }
        throw new Exception("Could not add room");
    }

    [Authorize]
    [HttpPost("watch")]
    public WatchResponse Watch(WatchParameter parameter)
    {
        var room = _roomRepository.GetRoomByName(parameter.RoomName);
        if(AuthorizeRoomSpectator(room))
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
            return response;
        }

        throw new Exception("Can not view pending cameras");

    }

    [Authorize(AuthenticationSchemes = "ApiAuthenticationScheme")]
    [HttpDelete("{roomName}")]
    public IActionResult DeleteRoom(string roomName)
    {
        var userLogin = Request.GetUserLoginFromToken();
        var roomToDelete = _roomRepository.GetRoomByName(roomName);
        if (roomToDelete is not null
            && roomToDelete.OwnerLogin == userLogin)
        {
            if (_roomRepository.DeleteRoom(roomToDelete))
            {
                return Ok("Room has been succesfully removed");
            }
        }
        return BadRequest("Could not delete room");
    }

    [Authorize(AuthenticationSchemes = "ApiAuthenticationScheme")]
    [HttpGet]
    public GetAllRoomsResponse GetAllRooms()
    {
        var login = Request.GetUserLoginFromToken();
        var user = _userRepository.GetUser(login);
        if (user is not null)
        {
            return new()
            {
                Rooms = WithoutPasswordConverter.RoomCollectionConverter(user.Rooms)
            };
        }
        throw new Exception("User does not exist in database");
    }

    [Authorize(AuthenticationSchemes = "ApiAuthenticationScheme")]
    [HttpGet("{roomName}")]
    public GetPendingCamerasResponse GetPendingCameras(string roomName)
    {
        var login = Request.GetUserLoginFromToken();
        var user = _userRepository.GetUser(login);
        var room = _roomRepository.GetRoomByName(roomName);
        if (user is not null 
            && room is not null
            && room.OwnerLogin == login)
        {
            var response = new GetPendingCamerasResponse();
            foreach (var camera in room.Cameras)
            {
                if (camera.AcceptationState == false)
                {
                    response.PendingCameras.Add(camera);
                }
            }
            return response;
        }
        throw new Exception("Cannot view pending cameras");
    }

    [Authorize]
    [HttpGet("recordings/{roomName}")]
    public GetRecordingsResponse GetRecordings(string roomName)
    {
        var room = _roomRepository.GetRoomByName(roomName);
        if (AuthorizeRoomSpectator(room))
        {
            var recordings = WithoutPasswordConverter.RecordingCollectionConverter(
                _recordingRepository.GetRoomRecordings(room));
            return new()
            {
                Recordings = recordings
            };
        }
        throw new Exception("Can not view recordings");
    }

    [Authorize(AuthenticationSchemes = "ApiAuthenticationScheme")]
    [HttpGet("qrCode/{roomName}")]
    public IActionResult GenerateQRCode(string roomName)
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
            return GenerateQRCodePngResponse(qrBodyString, qrCodeStreamName);
        }
        throw new Exception("You are not an owner of this room");
    }
    
    

    [AllowAnonymous]
    [HttpPost("token")]
    public GenerateTokenResponse GenerateToken(GenerateTokenParameter parameter)
    {
        var room = AuthenticateRoom(parameter);
        if (room is not null)
        {
            var token = GenerateRoomToken(room);
            var refreshToken = GenerateRefreshToken(room);
            SetRefreshToken(refreshToken, room.RoomName);
            return new()
            {
                AccessToken = token
            };
        }
        throw new RequestFailedException("Authorization failed");
    }

    [AllowAnonymous]
    [HttpPost("refreshToken/{roomName}")]
    public ActionResult<RefreshRoomTokenResponse> RefreshToken(string roomName)
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var roomNameFromCookie = JwtSecurityTokenExtension.GetClaim(refreshToken, "RoomName");
        if (Request.GetRoomNameFromToken() != roomNameFromCookie || roomNameFromCookie != roomName)
        {
            return BadRequest("Different rooms in access token and refresh token.");
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
        return BadRequest("Refresh token invlaid.");
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
    
    private RoomModel? AuthenticateRoom(GenerateTokenParameter room)
    {
        var roomFromDb = _roomRepository.GetRoom(room.RoomName, room.Password);
        return roomFromDb;
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

    private IActionResult GenerateQRCodePngResponse(string qrContent, string fileName)
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
            tokenHandler.ValidateToken(refreshToken, tokenValidationParameters, out SecurityToken validatedToken);
            return true;
        }
        catch (SecurityTokenValidationException ex)
        {
            return false;
        }
    }
}