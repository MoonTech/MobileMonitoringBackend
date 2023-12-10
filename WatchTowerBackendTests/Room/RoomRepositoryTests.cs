using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WatchTowerAPI.BusinessLogical.Repositories.RoomRepository;
using WatchTowerAPI.BusinessLogical.Repositories.UserRepository;
using WatchTowerAPI.DataAccess.DbContexts;
using WatchTowerAPI.Domain.Models;
using WatchTowerBackendTests.Utils;

namespace WatchTowerBackendTests.Room;

public class RoomRepositoryTests
{
    private readonly IRoomRepository _roomRepository;

    public RoomRepositoryTests()
    {
        _roomRepository = RoomRepositoryMock.SetRoomRepository();
    }

    [Fact]
    public void CreateRoomShouldReturnRoom()
    {
        var result = _roomRepository.CreateRoom("RoomName", "RoomPassword", DefaultUser());
        Assert.True(result is not null);
    }

    [Fact]
    public void GetRoomShouldReturnRoom()
    {
        _roomRepository.CreateRoom("RoomName", "RoomPassword", DefaultUser());
        var result = _roomRepository.GetRoom("RoomName", "RoomPassword");
        Assert.True(result is not null);
    }

    [Fact]
    public void GetRoomByNameShouldReturnRoom()
    {
        _roomRepository.CreateRoom("RoomName", "RoomPassword", DefaultUser());
        var result = _roomRepository.GetRoomByName("RoomName");
        Assert.True(result is not null);
    }
    
    [Fact]
    public void DeleteRoomShouldReturnTrueWhenRoomExists()
    {
        var room = _roomRepository.CreateRoom("RoomName", "RoomPassword", DefaultUser());
        var result = _roomRepository.DeleteRoom(room!);
        Assert.True(result);
    }
    
    [Fact]
    public void DeleteRoomShouldThrowExceptionWhenRoomDoesNotExist()
    {
        try
        {
            _roomRepository.DeleteRoom(new RoomModel()
            {
                RoomName = "NonExistingRoom"
            });
        }
        catch
        {
            Assert.True(true);
        }
    }

    [Fact]
    public void CheckRoomAndPasswordShouldReturnTrueWhenCorrect()
    {
        _roomRepository.CreateRoom("RoomName", "RoomPassword", DefaultUser());
        var result = _roomRepository.CheckRoomAndPassword("RoomName", "RoomPassword");
        Assert.True(result);
    }
    
    [Fact]
    public void CheckRoomAndPasswordShouldReturnFalseWhenIncorrect()
    {
        _roomRepository.CreateRoom("RoomName", "RoomPassword", DefaultUser());
        var result = _roomRepository.CheckRoomAndPassword("RoomName", "WrongPassword");
        Assert.True(!result);
    }

    private UserModel DefaultUser()
    {
        return new UserModel()
        {
            Login = "login",
            Password = "Password"
        };
    }
}

public static class RoomRepositoryMock
{
    internal static IRoomRepository SetRoomRepository()
    {
        var mockDbContext = RepositoryMockTest.CreateMockDbContext();
        var roomRepository = new RoomRepository(mockDbContext);
        roomRepository.CreateRoom("NewRoom", "NewPassword", new UserModel()
        {
            Login = "NewUserLogin",
            Password = "NewUserPassword"
        });
        return roomRepository;
    }
}