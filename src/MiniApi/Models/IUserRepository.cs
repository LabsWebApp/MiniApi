namespace MiniApi.Models;

public interface IUserRepository : IDisposable
{
    Task<List<User>> GetUsersAsync();
    Task<List<User>> GetUsersAsync(string name);
    Task<List<User>> GetUsersAsync(NumberInfo info);
    Task<User?> GetUserAsync(int id);
    Task CreateUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(int id);
    Task SaveAsync();
}