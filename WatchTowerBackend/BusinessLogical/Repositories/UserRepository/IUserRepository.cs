using WatchTowerBackend.BusinessLogical.Authentication;
using WatchTowerBackend.Domain.Models;

namespace WatchTowerBackend.BusinessLogical.Repositories.UserRepository;

public interface IUserRepository
{
    public UserModel AddUser(string login, string password);
    public UserModel GetUser(string login, string password);
    public UserModel GetUser(string login);
}