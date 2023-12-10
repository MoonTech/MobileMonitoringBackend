using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WatchTowerAPI.BusinessLogical.Repositories.UserRepository;
using WatchTowerAPI.Domain.Models;
using WatchTowerBackend.BusinessLogical.Authentication;
using WatchTowerBackend.Contracts.DTOs.Parameters.User;
using WatchTowerBackend.Contracts.DTOs.Responses.User;

namespace WatchTowerAPI.Presentation.Controllers;

[ApiController]
[Route("/[controller]")]
public class userController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _config;

    public userController(IUserRepository userRepository,
        IConfiguration config)
    {
        _userRepository = userRepository;
        _config = config;
    }

    // Endpoints
    [HttpPost]
    [AllowAnonymous]
    public SignUpUserResponse SignUp(SignUpUserParameter parameter)
    {
        var newUser = _userRepository.AddUser(parameter.Login, parameter.Password);
        if (newUser is not null)
        {
            return new()
            {
                AccessToken = GenerateUserToken(newUser)
            };
        }
        throw new Exception("Can not add user");
    }
    
    [AllowAnonymous]
    [HttpPost("login")]
    public LoginUserResponse Login(LoginUserParameter parameter)
    {
        var user = AuthenticateUser(parameter);
        if (user != null)
        {
            var token = GenerateUserToken(user);
            return new()
            {
                AccessToken = token
            };
        }
        throw new RequestFailedException("Authorization failed");
    }

    [Authorize(AuthenticationSchemes = "ApiAuthenticationScheme")]
    [HttpDelete("logout")]
    public IActionResult Logout()
    {
        throw new NotImplementedException();
    }
    
    // Additional Methods
    private string GenerateUserToken(UserModel user)
    {
        return JwtSecurityTokenExtension.GenerateToken(
            _config, 
            "Jwt:ApiKey",
            new[]
            {
                new Claim("login", user.Login),
                new Claim("type", "TokenAPI")
            });
    }
    private UserModel? AuthenticateUser(LoginUserParameter user)
    {
        var userFromDb = _userRepository.GetUser(user.Login, user.Password);
        return userFromDb;
    }
    
}