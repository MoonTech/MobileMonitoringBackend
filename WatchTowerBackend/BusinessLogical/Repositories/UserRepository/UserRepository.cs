using Microsoft.EntityFrameworkCore;
using WatchTowerBackend.BusinessLogical.Authentication;
using WatchTowerBackend.BusinessLogical.Utils;
using WatchTowerBackend.DataAccess.DbContexts;
using WatchTowerBackend.Domain.Models;

namespace WatchTowerBackend.BusinessLogical.Repositories.UserRepository;

public class UserRepository : BaseRepository, IUserRepository
{
    public UserRepository(WatchTowerDbContext context) : base(context) {}
    
    public UserModel? AddUser(string login, string password)
    {
        var newUser = new UserModel()
        {
            Login = login,
            Password = PasswordHash.HashPassword(password)
        };
        context.Users.Add(newUser);
        if (SaveChanges())
        {
            return newUser;
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