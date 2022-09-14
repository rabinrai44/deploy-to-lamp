using deploy_to_linux.Core.Entities.Entities;
using deploy_to_linux.Core.Interfaces;
using deploy_to_linux.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace deploy_to_linux.Infrastructure.Services;
public class UserService : IUserService
{
    private AppDbContext _context;

    public UserService(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        return await _context.Users.ToListAsync();
    }

    public User GetById(int id)
    {
        return _context.Users.Find(id);
    }

    public void Create(User user)
    {
        // save user
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public void UpdateAsync(User user)
    {
        _context.Users.Update(user);
        _context.SaveChanges();
    }

    public void DeleteAsync(User user)
    {
        _context.Users.Remove(user);
        _context.SaveChanges();
    }

    // helper methods

    private User getUser(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null) throw new KeyNotFoundException("User not found");
        return user;
    }

    public bool CheckEmailExists(string email)
    {
        return _context.Users.Any(x => x.Email == email);
    }
}