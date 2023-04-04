using MiniApi.Models.Entities.Events;

namespace MiniApi.Models.Repositories;

public interface IEventRepository : IDisposable
{
    Task<List<Event>> GetEventsAsync();
    Task<List<Event>> GetEventsAsync(string name);
    Task<List<Event>> GetEventsAsync(NumberInfo info);
    Task<Event?> GetEventAsync(int id);
    Task CreateEventAsync(Event @event);
    Task UpdateEventAsync(Event @event);
    Task DeleteEventAsync(int id);
    Task SaveAsync();
}