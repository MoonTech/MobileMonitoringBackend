using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Web.Http.Results;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WatchTowerBackend.BusinessLogical.Authentication;
using WatchTowerBackend.BusinessLogical.Repositories.UserRepository;
using WatchTowerBackend.BusinessLogical.Utils;
using WatchTowerBackend.BusinessLogical.Utils.Exceptions;
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
    private readonly int _tokenValidHours = 1;
    private readonly int _refreshTokenValidHours = 720;
    
    public userController(IUserRepository userRepository,
        IConfiguration config)
    {
        _userRepository = userRepository;
        _config = config;
    }

    // Endpoints
    [HttpPost]
    [AllowAnonymous]
    public ActionResult<SignUpUserResponse> SignUp(SignUpUserParameter parameter)
    {
        try
        {
            var newUser = _userRepository.AddUser(parameter.Login, parameter.Password);
            var refreshToken = GetRefreshToken(newUser);
            SetRefreshToken(refreshToken);
            SignUpUserResponse result = new()
            {
                AccessToken = GenerateUserToken(newUser)
            };
            return Ok(result);
        }
        catch (ObjectAlreadyExistsInDbException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [AllowAnonymous]
    [HttpPost("login")]
    public ActionResult<LoginUserResponse> Login(LoginUserParameter parameter)
    {
        try
        {
            var user = AuthenticateUser(parameter);
            var token = GenerateUserToken(user);
            var refreshToken = GetRefreshToken(user);
            SetRefreshToken(refreshToken);
            LoginUserResponse result = new()
            {
                AccessToken = token
            };
            return Ok(result);
        }
        catch (ObjectDoesNotExistInDbException ex)
        {
            return NotFound(ex.Message);
        }
        catch (WrongPasswordException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message); 
        }
    }

    [AllowAnonymous]
    [HttpPost("refreshToken")]
    public ActionResult<RefreshTokenResponse> RefreshToken()
    {
        try
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (refreshToken is null)
            {
                return BadRequest("No refresh token passed in cookies or refresh token expired");
            }
            
            var userLogin = JwtSecurityTokenExtension.GetClaim(refreshToken, "login");
            if (userLogin is null)
            {
                return BadRequest("No user login inside authorization token");
            }

            if (Request.GetUserLoginFromToken() != userLogin)
            {
                return BadRequest("Different users in access token and refresh token.");
            }

            var user = _userRepository.GetUser(userLogin);
            if (ValidateRefreshToken(refreshToken))
            {
                var newRefreshToken = GetRefreshToken(user);
                SetRefreshToken(newRefreshToken);
                string token = GenerateUserToken(user);
                var result = new RefreshTokenResponse()
                {
                    accessToken = token
                };
                return Ok(result);
            }
            return BadRequest("Invalid refresh token");
        }
        catch (ObjectDoesNotExistInDbException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {   
            return StatusCode(500, ex.Message);
        }
    }

    // Additional Methods
    private string GenerateUserToken(UserModel user)
    {
        return JwtSecurityTokenExtension.GenerateToken(
            _config, 
            "Jwt:ApiKey",
            _tokenValidHours,
            new[]
            {
                new Claim("login", user.Login),
                new Claim("type", "TokenAPI")
            });
    }
    
    private UserModel AuthenticateUser(LoginUserParameter user)
    {
        var userFromDb = _userRepository.GetUser(user.Login, user.Password);
        return userFromDb;
    }
    
    private RefreshToken GetRefreshToken(UserModel user)
    {
        var refreshToken = new RefreshToken
        {
            Token = GenerateRefreshToken(user),
            Expires = DateTime.Now.AddHours(_refreshTokenValidHours),
            Created = DateTime.Now
        };
        return refreshToken;
    }

    private void SetRefreshToken(RefreshToken newRefreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = newRefreshToken.Expires,
            Path = "/user/refreshToken"
        };
        Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);
    }
    
    private string GenerateRefreshToken(UserModel user)
    {
        return JwtSecurityTokenExtension.GenerateToken(
            _config,
            "Jwt:ApiRefreshKey",
            _refreshTokenValidHours,
            new[]
            {
                new Claim("login", user.Login),
                new Claim("type", "RefreshTokenAPI")
            });
    }
    
    private bool ValidateRefreshToken(string refreshToken)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters tokenValidationParameters = Constants.TokenValidationParameters(
                _config, "Jwt:ApiRefreshKey");
            tokenHandler.ValidateToken(refreshToken, tokenValidationParameters, out SecurityToken validatedToken);
            return true;
        }
        catch (SecurityTokenValidationException)
        {
            return false;
        }
    }
}