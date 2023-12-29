using System.Security.Claims;
using System.Security.Cryptography;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WatchTowerBackend.BusinessLogical.Authentication;
using WatchTowerBackend.BusinessLogical.Repositories.UserRepository;
using WatchTowerBackend.Contracts.DTOs.Parameters.User;
using WatchTowerBackend.Contracts.DTOs.Responses.User;
using WatchTowerBackend.Domain.Models;

namespace WatchTowerBackend.Presentation.Controllers;

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
        var refreshToken = GenerateRefreshToken();
        var newUser = _userRepository.AddUser(parameter.Login, parameter.Password, refreshToken);
        if (newUser is not null)
        {
            SetRefreshToken(newUser, refreshToken);
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
            var refreshToken = GenerateRefreshToken();
            SetRefreshToken(user, refreshToken);
            return new()
            {
                AccessToken = token
            };
        }
        throw new RequestFailedException("Authorization failed");
    }

    [Authorize(AuthenticationSchemes = "ApiAuthenticationScheme")]
    [HttpPost("refreshToken")]
    public ActionResult<string> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var userLogin = Request.GetUserLoginFromToken();
        var user = _userRepository.GetUser(userLogin);
        if (user is null)
        {
            return Unauthorized("User does not exist");
        }
        if (!user.RefreshToken.Equals(refreshToken))
        {
            return Unauthorized("Invalid Refresh Token");
        }
        if(user.TokenExpires < DateTime.Now)
        {
            return Unauthorized("Token expired");
        }

        string token = GenerateUserToken(user);
        var newRefreshToken = GenerateRefreshToken();
        SetRefreshToken(user, newRefreshToken);

        return Ok(token);
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
    
    private RefreshToken GenerateRefreshToken()
    {
        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.Now.AddHours(1),
            Created = DateTime.Now
        };
        return refreshToken;
    }

    private void SetRefreshToken(UserModel user, RefreshToken newRefreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = newRefreshToken.Expires
        };
        Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);
        _userRepository.SetRefreshToken(user, newRefreshToken);
    }
}