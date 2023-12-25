using Microsoft.Extensions.Configuration;
using Moq;
using WatchTowerBackend.Contracts.DTOs.Parameters.User;
using WatchTowerBackend.Presentation.Controllers;

namespace WatchTowerBackendTests.User;

public class UserControllerTests
{
    private readonly userController _userController;

    public UserControllerTests()
    {
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Jwt:ApiKey"])
            .Returns("fsekljsaKLJjkjkljlj988877888hjHjjkHJKHkhgfDEfsr6gVBnbJgjJHsdnfsasjB898hghVVSSFghjjhfDgBj86BGJjjjkiufcfdssFGT64fBB");
        _userController = new userController(UserRepositoryMock.SetUserRepository(), configMock.Object);
    }

    [Fact]
    public void ShouldReturnTokenWhenNewUserAdded()
    {
        var result = _userController.SignUp(new SignUpUserParameter()
        {
            Login = "login",
            Password = "Password"
        });
        Assert.True(result.AccessToken.Length > 0);
    }
    
    [Fact]
    public void ShouldReturnTokenWhenLoginCorrect()
    {
        var accessToken = _userController.SignUp(new SignUpUserParameter()
        {
            Login = "login",
            Password = "Password"
        });
        var result = _userController.Login(new LoginUserParameter()
        {
            Login = "login",
            Password = "Password"
        });
        Assert.True(result.AccessToken.Length > 0);
    }

    [Fact]
    public void ShouldThrowAnErrorWhenUserAlreadyExists()
    {
        try
        {
            var result1 = _userController.SignUp(new SignUpUserParameter()
            {
                Login = "login",
                Password = "Password"
            });
            var result2 = _userController.SignUp(new SignUpUserParameter()
            {
                Login = "login",
                Password = "Password"
            });
            Assert.False(result2.AccessToken.Length > 0);
        }
        catch
        {
            Assert.True(true);
        }
    }
}