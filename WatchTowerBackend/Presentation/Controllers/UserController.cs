using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WatchTowerAPI.BusinessLogical.Repositories.UserRepository;
using WatchTowerAPI.Domain.Models;
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
                AccessToken = GenerateToken(newUser)
            };
        }
        throw new Exception("Can not add user");
    }
    
    [AllowAnonymous]
    [HttpPost("login")]
    public LoginUserResponse Login(LoginUserParameter parameter)
    {
        var user = Authenticate(parameter);
        if (user != null)
        {
            var token = GenerateToken(user);
            return new()
            {
                AccessToken = token
            };
        }
        throw new RequestFailedException("Authorization failed");
    }

    [Authorize]
    [HttpDelete("logout")]
    public IActionResult Logout()
    {
        throw new NotImplementedException();
    }
    
    // Additional Methods
    private string GenerateToken(UserModel user) // TODO Improve token generation
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim("login",user.Login),
            new Claim("role", "normal user") //TODO Should we add roles to our database?
        };
        
        var token = new JwtSecurityToken(_config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credentials);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    private UserModel? Authenticate(LoginUserParameter user)
    {
        var userFromDb = _userRepository.GetUser(user.Login, user.Password);
        return userFromDb;
    }
}