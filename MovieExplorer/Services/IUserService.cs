using MovieExplorer.Models;
namespace MovieExplorer.Services
{
    public interface IUserService
    {
        Task<User> Register(string username,string password,string email);
        Task<User?> Login(string username,string password);
    }
}
