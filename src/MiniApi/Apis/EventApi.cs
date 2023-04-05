using MiniApi.Models.Entities.Events;
using MiniApi.ResponseFormats;

namespace MiniApi.Apis;

public class EventApi : IApi
{
    public void Register(WebApplication app)
    {
        app.MapGet("/events", Get)
            .Produces<List<Event>>()
            .WithName("AllEvents")
            .WithTags("Getters");

        app.MapGet("/events/{Id}", GetById)
            .Produces<Event>()
            .WithName("Event")
            .WithTags("Getters");

        app.MapGet("/events/search/name/{query}", SearchByName)
            .Produces<List<Event>>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("SearchByNameEvents")
            .WithTags("Getters");

        app.MapGet("/events/search/numbers/{query}", SearchByInfo)
            .Produces<List<User>>()
            .Produces(StatusCodes.Status404NotFound)
            .ExcludeFromDescription();

        app.MapPost("/events",Post)
            .Accepts<Event>("application/json")
            .Produces<Event>(StatusCodes.Status201Created)
            .WithName("CreateEvent")
            .WithTags("Creators");

        app.MapPut("/events", Put)
            .Accepts<Event>("application/json")
            .WithName("UpdateEvent")
            .WithTags("Updaters");

        app.MapDelete("/events/{Id}", Delete)
            .WithName("DeleteEvent")
            .WithTags("Deleters");
    }

    [AllowAnonymous]
    private async Task<IResult> Get(IEventRepository repository) =>
        Results.Extensions.Xml(await repository.GetEventsAsync());

    [AllowAnonymous]
    private async Task<IResult> SearchByName(string query, IEventRepository repository) =>
        await repository.GetEventsAsync(query) is { } events
            ? Results.Ok(events)
            : Results.NotFound(Array.Empty<Event>());

    [AllowAnonymous]
    private async Task<IResult> SearchByInfo(NumberInfo query, IEventRepository repository) =>
        await repository.GetEventsAsync(query) is { } events
            ? Results.Ok(events)
            : Results.NotFound(Array.Empty<Event>());

    [AllowAnonymous]
    private async Task<IResult> GetById(int id, IEventRepository repository) =>
        await repository.GetEventAsync(id) is { } @event
            ? Results.Ok(@event)
            : Results.NotFound();

    [AllowAnonymous]
    private async Task<IResult> Post([FromBody] Event @event, IEventRepository repository)
    {
        await repository.CreateEventAsync(@event);
        await repository.SaveAsync();
        return Results.Created($"/hotels/{@event.Id}", @event);
    }

    [AllowAnonymous]
    private async Task<IResult> Put([FromBody] Event @event, IEventRepository repository)
    {
        await repository.UpdateEventAsync(@event);
        await repository.SaveAsync();
        return Results.NoContent();
    }

    [AllowAnonymous]
    private async Task<IResult> Delete(int id, IEventRepository repository)
    {
        await repository.DeleteEventAsync(id);
        await repository.SaveAsync();
        return Results.NoContent();
    }
}