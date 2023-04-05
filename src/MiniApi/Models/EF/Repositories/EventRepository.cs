using MiniApi.Models.Entities.Events;

namespace MiniApi.Models.EF.Repositories;

public class EventRepository : IEventRepository
{
    private readonly ApiDb _context;

    public EventRepository(ApiDb context) => _context = context;

    public Task<List<Event>> GetEventsAsync() => _context.Events.ToListAsync();

    public Task<List<Event>> GetEventsAsync(string name) => _context.Events
        .Where(u => u.Name.ToLower().Contains(name.ToLower())).ToListAsync();

    public Task<List<Event>> GetEventsAsync(NumberInfo info) =>
        _context.Events.Where(u => u.Code == info.Code && Math.Abs(u.Ratio - info.Ratio) < 0.1).ToListAsync();

    public async Task<Event?> GetEventAsync(int id) => await _context.Events.FindAsync(id);

    public async Task CreateEventAsync(Event @event) => await _context.Events.AddAsync(@event);

    public async Task UpdateEventAsync(Event @event)
    {
        var oldEvent = await _context.Events.FindAsync(@event.Id);
        if (oldEvent == null) return;
        if (oldEvent.Name != @event.Name) oldEvent.Name = @event.Name;
        if (oldEvent.Code != @event.Code) oldEvent.Code = @event.Code;
        if (Math.Abs(oldEvent.Ratio - @event.Ratio) > 0.0001) oldEvent.Ratio = @event.Ratio;
    }

    public async Task DeleteEventAsync(int id)
    {
        var oldEvent = await _context.Events.FindAsync(id);
        if (oldEvent == null) return;
        _context.Events.Remove(oldEvent);
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