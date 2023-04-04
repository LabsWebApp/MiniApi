var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApiDb>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));

builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApiDb>();
    db.Database.EnsureCreated();
}

app.MapGet("/events", [AllowAnonymous] async (IEventRepository repo) => Results
     //   .Ok(await repo.GetEventsAsync()))
        .Extensions.Xml(await repo.GetEventsAsync()))
    .Produces<List<Event>>()
    .WithName("AllEvents")
    .WithTags("Getters");

app.MapGet("/login", [AllowAnonymous] async (HttpContext context,
    ITokenService tokenService, IUserRepository userRepository) => await Task.Run(() => 
{
    // Так нельзя, для теста!!!
    User user = new()
    {
        Name = context.Request.Query["username"]!,
        Password = context.Request.Query["password"]!
    };
    var userDto = userRepository.GetUser(user);
    //if (userDto == null) return Results.Unauthorized();
    var token = tokenService.BuildToken(
        builder.Configuration["Jwt:Key"]!,
        builder.Configuration["Jwt:Issuer"]!, userDto);
    return Results.Ok(token);
}));

app.MapGet("/events/{Id}", [AllowAnonymous] async (int id, IEventRepository repo) => Results
        .Ok(await repo.GetEventAsync(id)) is { } @event
        ? Results.Ok(@event)
        : Results.NotFound())
    .Produces<Event>()
    .WithName("Event")
    .WithTags("Getters");
//.ExcludeFromDescription(); 

app.MapGet("/events/search/name/{query}",
        [AllowAnonymous] async (string query, IEventRepository repository) =>
            await repository.GetEventsAsync(query) is {} events && events.Any()
                ? Results.Ok(events)
                : Results.NotFound(Array.Empty<Event>()))
    .Produces<List<Event>>()
    .Produces(StatusCodes.Status404NotFound)
    .WithName("SearchEvents")
    .WithTags("Getters");

app.MapGet("/events/search/numbers/{query}",
        [AllowAnonymous] async (NumberInfo query, IEventRepository repository) =>
            await repository.GetEventsAsync(query) is { } events && events.Any()
                ? Results.Ok(events)
                : Results.NotFound(Array.Empty<Event>()))
    .Produces<List<Event>>()
    .Produces(StatusCodes.Status404NotFound)
    .ExcludeFromDescription();


app.MapPost("/events", 
        [AllowAnonymous] async ([FromBody] Event @event, IEventRepository repo) =>
{
    await repo.CreateEventAsync(@event);
    await repo.SaveAsync();
    return Results.Created($"/events/{@event.Id}", @event);
}).Accepts<Event>("application/json")
    .Produces<Event>(StatusCodes.Status201Created)
    .WithName("CreateEvent")
    .WithTags("Creators");

app.MapPut("/events", [AllowAnonymous] async ([FromBody] Event @event, IEventRepository repo) =>
{
    await repo.UpdateEventAsync(@event);
    await repo.SaveAsync();
    return Results.NoContent();
}).Accepts<Event>("application/json")
    .WithName("UpdateEvent")
    .WithTags("Updaters");

app.MapDelete("/events/{Id}", 
        [AllowAnonymous] async (int id, IEventRepository repo) =>
{
    await repo.DeleteEventAsync(id);
    await repo.SaveAsync();
    return Results.NoContent();
}).WithName("DeleteEvent")
    .WithTags("Deleters");

// Конвейер обработки запроса - middleware 
app.UseHttpsRedirection();

app.Run();