using Microsoft.EntityFrameworkCore;
using MovieExplorer.Data;
using MovieExplorer.Models;
using System.Diagnostics;

namespace MovieExplorer.Services
{
    public class UserService(MovieExplorerDbContext dbContext) : IUserService
    {
        
        public async Task<User> Login(string email, string password)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == email);
           if(null == user || false==PasswordHasher.VerifyPassword(password,user.PasswordHash))
            {
                return null;
            }
           return user;
        }

        public async Task<User> Register(string username, string password, string email)
        {
            if (await dbContext.Users.AnyAsync(u => u.Email == email))
            {
                throw new InvalidOperationException("Email already exists!");
            }

            var user = new User
            {
                UserName = username,
                Email = email,
                PasswordHash = PasswordHasher.HashPasword(password),
            };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
            return user;
        }
    }
}
