using Microsoft.EntityFrameworkCore;
using MobileMonitoringBackend.BusinessLogical.Authentication;
using MobileMonitoringBackend.BusinessLogical.Utils;
using MobileMonitoringBackend.BusinessLogical.Utils.Exceptions;
using MobileMonitoringBackend.BusinessLogical.Utils.Exceptions.User;
using MobileMonitoringBackend.DataAccess.DbContexts;
using MobileMonitoringBackend.Domain.Models;

namespace MobileMonitoringBackend.BusinessLogical.Repositories.UserRepository;

public class UserRepository : BaseRepository, IUserRepository
{
    public UserRepository(MobileMonitoringDbContext context) : base(context) {}
    
    public UserModel AddUser(string login, string password)
    {
        var newUser = new UserModel()
        {
            Login = login,
            Password = PasswordHash.HashPassword(password)
        };
        try
        {
            context.Users.Add(newUser);
            if (SaveChanges())
            {
                return newUser;
            }
            throw new CouldNotSaveChangesException();
        }
        catch (Exception)
        {
            GetUser(login);
            throw new UserAlreadyExistsException(login);
        }
    }

    public UserModel GetUser(string login, string password)
    {
        var user = context.Users.Include("Rooms.Cameras").SingleOrDefault(user => user.Login == login);
        if (user is not null && PasswordHash.VerifyPassword(password, user.Password))
        {
            return user;
        }
        GetUser(login);
        throw new UserPasswordWrongException(login);
    }

    public UserModel GetUser(string login)
    {
        var user = context.Users.Include("Rooms.Cameras").SingleOrDefault(user => user.Login == login);
        if (user is not null)
        {
            return user;
        }
        throw new UserDoesNotExistException(login);
    }
}