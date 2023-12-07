using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using Azure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using WatchTowerAPI.BusinessLogical.Repositories.RoomRepository;
using WatchTowerAPI.BusinessLogical.Repositories.UserRepository;
using WatchTowerAPI.Contracts.DTOs.Parameters;
using WatchTowerAPI.Contracts.DTOs.Parameters.Room;
using WatchTowerAPI.Contracts.DTOs.Responses;
using WatchTowerAPI.Contracts.DTOs.Responses.Room;
using WatchTowerAPI.DataAccess.DbContexts;
using WatchTowerAPI.Domain.Models;
using WatchTowerBackend.BusinessLogical.Authentication;
using WatchTowerBackend.BusinessLogical.Converters;
using WatchTowerBackend.Contracts.DTOs.ModelsWithoutPasswords;
using WatchTowerBackend.Contracts.DTOs.Parameters.Room;
using WatchTowerBackend.Contracts.DTOs.Parameters.User;


namespace WatchTowerAPI.Presentation.Controllers;

[ApiController]
[Route("/room")]
public class roomController : ControllerBase
{
    private readonly IRoomRepository _roomRepository;
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _config;

    public roomController(IRoomRepository roomRepository,
        IUserRepository userRepository,
        IConfiguration config)
    {
        _roomRepository = roomRepository;
        this._userRepository = userRepository;
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
        var userLogin = Request.GetUserLoginFromToken();
        var roomName = Request.GetRoomNameFromToken();
        if ((room is not null && userLogin is not null && userLogin == room.OwnerLogin)
            || (roomName is not null))
        {
            var response = new WatchResponse()
            {
                RoomName = room.RoomName
            };
            foreach (var camera in room.Cameras)
            {
                if (camera.AcceptationState == true)
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
}