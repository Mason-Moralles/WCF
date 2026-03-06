using TaskManager.Core.Interfaces;
using TaskManager.Core.Models;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly TaskDbContext _db;

    public UserRepository(TaskDbContext db)
    {
        _db = db;
    }

    public UserInfo? GetById(int id) => _db.Users.Find(id);

    public UserInfo? GetByUsername(string username) =>
        _db.Users.FirstOrDefault(u => u.Username == username);

    public List<UserInfo> GetAll() => _db.Users.ToList();
}
