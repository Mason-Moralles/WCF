using TaskManager.Core.Models;

namespace TaskManager.Core.Interfaces;

public interface IUserRepository
{
    UserInfo? GetById(int id);
    UserInfo? GetByUsername(string username);
    List<UserInfo> GetAll();
}
