using MobileMonitoringBackend.BusinessLogical.Authentication;
using MobileMonitoringBackend.Domain.Models;

namespace MobileMonitoringBackend.BusinessLogical.Repositories.UserRepository;

public interface IUserRepository
{
    public UserModel AddUser(string login, string password);
    public UserModel GetUser(string login, string password);
    public UserModel GetUser(string login);
}