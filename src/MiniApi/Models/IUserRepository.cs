namespace MiniApi.Models;

public interface IUserRepository : IDisposable
{
    Task<List<User>> GetUsersAsync();
    Task<User?> GetUserAsync(int id);
    Task CreateUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(int id);
    Task SaveAsync();
}