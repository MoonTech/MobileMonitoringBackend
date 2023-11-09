using WatchTowerAPI.Domain.Models;

namespace WatchTowerAPI.BusinessLogical.Repositories.UserRepository;

public interface IUserRepository
{
    public UserModel? AddUser(string login, string password);
    public UserModel? GetUser(string login, string password);
}