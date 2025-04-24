using MovieExplorer.Models;
using MovieExplorer.Models.ViewModels;
namespace MovieExplorer.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> RegisterAsync(RegisterViewModel model);
        Task<User?> LoginAsync(string email, string password);
        Task<User> FindByIdAsync(int id);
    }
}
