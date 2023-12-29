using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WatchTowerBackend.BusinessLogical.Authentication;
using WatchTowerBackend.BusinessLogical.Repositories.UserRepository;
using WatchTowerBackendTests.Utils;

namespace WatchTowerBackendTests.User;

public class UserRepositoryTests
{
    private readonly IUserRepository _userRepositoryMock;
    private readonly RefreshToken _exampleRefreshToken;

    public UserRepositoryTests()
    {
        _userRepositoryMock = UserRepositoryMock.SetUserRepository();
        _exampleRefreshToken = new RefreshToken()
        {
            Token = "abcdef",
            Created = DateTime.Now,
            Expires = DateTime.Now.AddHours(1)
        };
    }
    
    [Fact]
    public void ShouldAddUser()
    {
        var newUser = _userRepositoryMock.AddUser("Login", "Password",_exampleRefreshToken);
        Assert.True(newUser is not null);
    }

    [Fact]
    public void ShouldGetUserByLogin()
    {
        _userRepositoryMock.AddUser("Login", "Password",_exampleRefreshToken);
        var userFromDb = _userRepositoryMock.GetUser("Login");
        Assert.True(userFromDb is not null);
    }
    
    [Fact]
    public void ShouldNotGetUserByLoginWhenNotInDb()
    {
        _userRepositoryMock.AddUser("Login", "Password",_exampleRefreshToken);
        var userFromDb = _userRepositoryMock.GetUser("WrongLogin");
        Assert.True(userFromDb is null);
    }

    [Fact]
    public void ShouldGetUserByLoginAndPassword()
    {
        _userRepositoryMock.AddUser("Login", "Password",_exampleRefreshToken);
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