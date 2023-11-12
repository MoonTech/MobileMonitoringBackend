using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
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

namespace WatchTowerAPI.Presentation.Controllers;

[ApiController]
[Route("/room")]
public class roomController : ControllerBase
{
    private readonly IRoomRepository _roomRepository;
    private readonly IUserRepository _userRepository;
    private readonly WatchTowerDbContext _dbContext;

    public roomController(IRoomRepository roomRepository,
        IUserRepository userRepository,
        WatchTowerDbContext dbContext)
    {
        _roomRepository = roomRepository;
        this._userRepository = userRepository;
        _dbContext = dbContext;
    }

    [Authorize]
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
    public WatchResponse Watch()
    {
        throw new NotImplementedException();
    }

    [Authorize]
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

    [Authorize]
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

    [Authorize]
    [HttpGet("{roomName}")]
    public GetPendingCamerasResponse GetPendingCameras(string roomName)
    {
        throw new NotImplementedException();
    }
}