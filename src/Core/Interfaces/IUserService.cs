using API.Entities;

namespace Core.Interfaces;

public interface IUserService
{
    Task<IEnumerable<User>> GetAll();
    User GetById(int id);
    bool CheckEmailExists(string email);
    void Create(User user);
    void UpdateAsync(User user);
    void DeleteAsync(User user);
}
