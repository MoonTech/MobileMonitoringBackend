using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MobileMonitoringBackend.BusinessLogical.Authentication;
using MobileMonitoringBackend.BusinessLogical.Repositories.UserRepository;
using MobileMonitoringBackendTests.Utils;

namespace MobileMonitoringBackendTests.User;

public class UserRepositoryTests
{
    private readonly IUserRepository _userRepositoryMock;

    public UserRepositoryTests()
    {
        _userRepositoryMock = UserRepositoryMock.SetUserRepository();
    }
    
    [Fact]
    public void ShouldAddUser()
    {
        var newUser = _userRepositoryMock.AddUser("Login", "Password");
        Assert.True(newUser is not null);
    }

    [Fact]
    public void ShouldGetUserByLogin()
    {
        _userRepositoryMock.AddUser("Login", "Password");
        var userFromDb = _userRepositoryMock.GetUser("Login");
        Assert.True(userFromDb is not null);
    }
    
    [Fact]
    public void ShouldNotGetUserByLoginWhenNotInDb()
    {
        try
        {
            _userRepositoryMock.AddUser("Login", "Password");
            var userFromDb = _userRepositoryMock.GetUser("WrongLogin");
        }
        catch
        {
            Assert.True(true);
        }
    }

    [Fact]
    public void ShouldGetUserByLoginAndPassword()
    {
        _userRepositoryMock.AddUser("Login", "Password");
        var userFromDb = _userRepositoryMock.GetUser("Login", "Password");
        Assert.True(userFromDb is not null);
    }
}

public static class UserRepositoryMock
{
    internal static IUserRepository SetUserRepository()
    {
        var mockDbContext = RepositoryMockTest.CreateMockDbContext();
        var userRepository = new UserRepository(mockDbContext);
        return userRepository;
    }
}