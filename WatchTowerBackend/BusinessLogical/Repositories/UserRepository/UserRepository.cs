using Microsoft.EntityFrameworkCore;
using WatchTowerBackend.BusinessLogical.Authentication;
using WatchTowerBackend.BusinessLogical.Utils;
using WatchTowerBackend.DataAccess.DbContexts;
using WatchTowerBackend.Domain.Models;

namespace WatchTowerBackend.BusinessLogical.Repositories.UserRepository;

public class UserRepository : BaseRepository, IUserRepository
{
    public UserRepository(WatchTowerDbContext context) : base(context) {}
    
    public UserModel? AddUser(string login, string password, RefreshToken refreshToken)
    {
        var newUser = new UserModel()
        {
            Login = login,
            Password = PasswordHash.HashPassword(password),
            RefreshToken = refreshToken.Token,
            TokenCreated = refreshToken.Created,
            TokenExpires = refreshToken.Expires
        };
        context.Users.Add(newUser);
        if (SaveChanges())
        {
            return newUser;
        }
        return null;
    }

    public UserModel? SetRefreshToken(UserModel user, RefreshToken refreshToken)
    {
        user.RefreshToken = refreshToken.Token;
        user.TokenCreated = refreshToken.Created;
        user.TokenExpires = refreshToken.Expires;
        if (SaveChanges())
        {
            return user;
        }
        return null;
    }

    public UserModel? GetUser(string login, string password)
    {
        var user = context.Users.SingleOrDefault(user => user.Login == login);
        if (PasswordHash.VerifyPassword(password, user.Password))
        {
            return user;
        }
        return null;
    }

    public UserModel? GetUser(string login)
    {
        var user = context.Users.Include("Rooms.Cameras").SingleOrDefault(user => user.Login == login);
        return user;
    }
}