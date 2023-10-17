using System.Data;
using Microsoft.AspNetCore.Mvc;
using WatchTowerAPI.BusinessLogical.Repositories.RoomRepository;
using WatchTowerAPI.Contracts.DTOs.Parameters;
using WatchTowerAPI.Contracts.DTOs.Parameters.Room;
using WatchTowerAPI.Contracts.DTOs.Responses;
using WatchTowerAPI.Contracts.DTOs.Responses.Room;

namespace WatchTowerAPI.Presentation.Controllers;

[ApiController]
[Route("/[controller]")]
public class RoomController : ControllerBase
{
    private readonly IRoomRepository _roomRepository;

    public RoomController(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }
    
    [HttpPost("Create")]
    public CreateResponse CreateRoom(CreateParameter parameter)
    {
        return new CreateResponse()
        {
            RoomId = _roomRepository.CreateRoom(parameter.Password)
        };
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
    }
}