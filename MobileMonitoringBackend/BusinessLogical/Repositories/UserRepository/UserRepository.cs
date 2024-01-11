using Microsoft.EntityFrameworkCore;
using MobileMonitoringBackend.BusinessLogical.Authentication;
using MobileMonitoringBackend.BusinessLogical.Utils;
using MobileMonitoringBackend.BusinessLogical.Utils.Exceptions;
using MobileMonitoringBackend.DataAccess.DbContexts;
using MobileMonitoringBackend.Domain.Models;

namespace MobileMonitoringBackend.BusinessLogical.Repositories.UserRepository;

public class UserRepository : BaseRepository, IUserRepository
{
    public UserRepository(MobileMonitoringDbContext context) : base(context) {}
    
    public UserModel? AddUser(string login, string password)
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
            throw new CouldNotSaveChangesException("Could not save changes in database");
        }
        catch (Exception)
        {
            GetUser(login);
            throw new ObjectAlreadyExistsInDbException($"User with login {login} already exists.");
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
        throw new WrongPasswordException($"Wrong password for user {login}");
    }

    public UserModel GetUser(string login)
    {
        var user = context.Users.Include("Rooms.Cameras").SingleOrDefault(user => user.Login == login);
        if (user is not null)
        {
            return user;
        }
        throw new ObjectDoesNotExistInDbException($"User {login} does not exist in database");
    }
}