using API.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
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
            // validate
            if (_context.Users.Any(x => x.Email == user.Email)) { }
            // throw new AppException("User with the email '" + model.Email + "' already exists");

            // // map model to new user object
            // var user = _mapper.Map<User>(model);

            // hash password
            // user.PasswordHash = BCrypt.HashPassword(model.Password);

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
}