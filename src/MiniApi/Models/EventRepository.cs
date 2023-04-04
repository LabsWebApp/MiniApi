namespace MiniApi.Models;

public class EventRepository : IEventRepository
{
    private readonly ApiDb _context;

    public EventRepository(ApiDb context) => _context = context;

    public Task<List<Event>> GetEventsAsync() => _context.Events.ToListAsync();

    public Task<List<Event>> GetEventsAsync(string name) =>
        _context.Events.Where(u => u.Name.ToLower().Contains(name.ToLower())).ToListAsync();
    
    public Task<List<Event>> GetEventsAsync(NumberInfo info) =>
        _context.Events.Where(u => u.Code == info.Age && Math.Abs(u.Ratio - info.Ratio) < 0.0001).ToListAsync();

    public async Task<Event?> GetEventAsync(int id) => await _context.Events.FindAsync(id);

    public async Task CreateEventAsync(Event @event) => await _context.Events.AddAsync(@event);

    public async Task UpdateEventAsync(Event @event)
    {
        var oldUser = await _context.Events.FindAsync(@event.Id);
        if (oldUser == null) return;
        if (oldUser.Name != @event.Name) oldUser.Name = @event.Name;
        if (oldUser.Code != @event.Code) oldUser.Code = @event.Code;
        if (Math.Abs(oldUser.Ratio - @event.Ratio) > 0.0001) oldUser.Ratio = @event.Ratio;
    }

    public async Task DeleteEventAsync(int id)
    {
        var oldUser = await _context.Events.FindAsync(id);
        if (oldUser == null) return;
        _context.Events.Remove(oldUser);
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