using deploy_to_linux.Core.Entities.Entities;

namespace deploy_to_linux.Core.Interfaces;

public interface IUserService
{
    Task<IEnumerable<User>> GetAll();
    User GetById(int id);
    bool CheckEmailExists(string email);
    void Create(User user);
    void UpdateAsync(User user);
    void DeleteAsync(User user);
}
