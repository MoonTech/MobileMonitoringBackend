using Microsoft.EntityFrameworkCore;
using WatchTowerAPI.DataAccess.DbContexts;
using WatchTowerAPI.Domain.Models;

namespace WatchTowerAPI.BusinessLogical.Repositories.UserRepository;

public class UserRepository : BaseRepository, IUserRepository
{
    public UserRepository(WatchTowerDbContext context) : base(context) {}
    
    public UserModel? AddUser(string login, string password)
    {
        var newUser = new UserModel()
        {
            Login = login,
            Password = password
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
        var user = context.Users.SingleOrDefault(user => user.Login == login && user.Password == password);
        return user;
    }

    public UserModel? GetUserByLogin(string login)
    {
        var user = context.Users.Include("Rooms.Cameras").SingleOrDefault(user => user.Login == login);
        return user;
    }
}