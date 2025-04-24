using Microsoft.EntityFrameworkCore;
using MovieExplorer.Data;
using MovieExplorer.Models;
using MovieExplorer.Models.ViewModels;
using MovieExplorer.Services.Interfaces;

namespace MovieExplorer.Services
{
    public class UserService(MovieExplorerDbContext dbContext) : IUserService
    {
        public async Task<User> FindByIdAsync(int id)
        {
            return await dbContext.Users.FindAsync(id);
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (null == user || false == PasswordHasher.VerifyPassword(password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException();
            }
            return user;
        }

        public async Task<User> RegisterAsync(RegisterViewModel model)
        {
            if (await dbContext.Users.AnyAsync(u => u.Email == model.Email))
            {
                throw new InvalidOperationException("Email already exists!");
            }

            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                PasswordHash = PasswordHasher.HashPasword(model.Password),
            };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
            return user;
        }
    }
}
