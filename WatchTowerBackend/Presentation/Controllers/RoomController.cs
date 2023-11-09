using System.Data;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using WatchTowerAPI.BusinessLogical.Repositories.RoomRepository;
using WatchTowerAPI.Contracts.DTOs.Parameters;
using WatchTowerAPI.Contracts.DTOs.Parameters.Room;
using WatchTowerAPI.Contracts.DTOs.Responses;
using WatchTowerAPI.Contracts.DTOs.Responses.Room;
using WatchTowerAPI.Domain.Models;
using WatchTowerBackend.Contracts.DTOs.Parameters.Room;

namespace WatchTowerAPI.Presentation.Controllers;

[ApiController]
[Route("/room")]
public class roomController : ControllerBase
{
    private readonly IRoomRepository _roomRepository;

    public roomController(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    //[Authorize]
    [HttpPost]
    public PostRoomResponse PostRoom(PostRoomParameter parameter)
    {
        var fullAuth = Request.Headers.Authorization;
        var parsedAuth = AuthenticationHeaderValue.Parse(fullAuth);
        var accessToken1 = parsedAuth.Parameter;
        var accessToken2 = Request.Headers[HeaderNames.Authorization];
        var accessToken3 = HttpContext.Request.Headers["Authorization"];
        return new()
        {
            RoomName = accessToken1
        };
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
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpGet]
    public GetAllRoomsResponse GetAllRooms()
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpGet("{roomName}")]
    public GetPendingCamerasResponse GetPendingCameras(string roomName)
    {
        throw new NotImplementedException();
    }
    
    
    /*[HttpPost("Create")]
    public PostRoomResponse CreateRoom(CreateParameter parameter)
    {
        return new PostRoomResponse()
        {
            RoomName = _roomRepository.CreateRoom(parameter.Password)
        };
    }
    
    [HttpGet("ExampleWithDb")]
    public RoomModel ExampleWithDb()
    {
        return _roomRepository.GetFirstRoom();
    }

    [HttpGet("Authorize")]
    public AuthorizeResponse AuthorizeRoom([FromQuery]AuthorizeParameter parameter)
    {
        if (_roomRepository.CheckRoomAndPassword(parameter.RoomId, parameter.Password))
        {
            // TODO Create normal token
            return new()
            {
                AccessToken = "AccessToken"
            };
        }
        else
        {
            throw new UnauthorizedAccessException("Login or Password wrong");
        }
    }*/
}