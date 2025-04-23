using MovieExplorer.Models;
namespace MovieExplorer.Services
{
    public interface IUserService
    {
        Task<User> RegisterAsync(string username,string password,string email);
        Task<User?> LoginAsync(string email,string password);
        Task<User> FindByIdAsync(int id);
    }
}
