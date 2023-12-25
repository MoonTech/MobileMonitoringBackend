using System.Security.Claims;
using System.Text.Json;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;
using SkiaSharp.QrCode.Image;
using WatchTowerBackend.BusinessLogical.Authentication;
using WatchTowerBackend.BusinessLogical.Converters;
using WatchTowerBackend.BusinessLogical.Repositories.RecordingRepository;
using WatchTowerBackend.BusinessLogical.Repositories.RoomRepository;
using WatchTowerBackend.BusinessLogical.Repositories.UserRepository;
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
        var user = _userRepository.GetUserByLogin(userLogin);
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
        var user = _userRepository.GetUserByLogin(login);
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
        var user = _userRepository.GetUserByLogin(login);
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
    [HttpPost("qrCode")]
    public IActionResult GenerateQRCodexd(GenerateQRCodeParameter parameter)
    {
        var room = _roomRepository.GetRoomByName(parameter.RoomName);
        var userLogin = Request.GetUserLoginFromToken();
        if (userLogin == room.OwnerLogin)
        {
            var qrBodyObject = new
            {
                token = GenerateRoomToken(room),
                roomName = room.RoomName
            };
            string qrBodyString = JsonSerializer.Serialize(qrBodyObject);
            var qrByteArray = GenerateQRbyteArray(qrBodyString);
            var qrCodeStreamName = room.RoomName + "_qr.png";
            return File(qrByteArray, "application/force-download", qrCodeStreamName);
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
            return new()
            {
                AccessToken = token
            };
        }
        throw new RequestFailedException("Authorization failed");
    }
    
    // Additional Methods
    private string GenerateRoomToken(RoomModel room)
    {
        return JwtSecurityTokenExtension.GenerateToken(
            _config,
            "Jwt:RoomKey",
            new[]
            {
                new Claim("RoomName", room.RoomName),
                new Claim("type", "RoomAuth")
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
}