namespace MiniApi.Models;

public class UserRepository : IUserRepository
{
    private readonly ApiDb _context;

    public UserRepository(ApiDb context) => _context = context;

    public Task<List<User>> GetUsersAsync() => _context.Users.ToListAsync();

    public async Task<User?> GetUserAsync(int id) => await _context.Users.FindAsync(id);

    public async Task CreateUserAsync(User user) => await _context.Users.AddAsync(user);

    public async Task UpdateUserAsync(User user)
    {
        var oldUser = await _context.Users.FindAsync(user.Id);
        if (oldUser == null) return;
        if (oldUser.Name != user.Name) oldUser.Name = user.Name;
        if (oldUser.Age != user.Age) oldUser.Age = user.Age;
        if (Math.Abs(oldUser.Ratio - user.Ratio) > 0.0001) oldUser.Ratio = user.Ratio;
    }

    public async Task DeleteUserAsync(int id)
    {
        var oldUser = await _context.Users.FindAsync(id);
        if (oldUser == null) return;
        _context.Users.Remove(oldUser);
    }

    public Task SaveAsync() => _context.SaveChangesAsync();

    private bool _disposed = false;

    private void Dispose(bool disposing)
    {
        if (!_disposed)
            if (disposing) _context.Dispose();
        _disposed = true;
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}