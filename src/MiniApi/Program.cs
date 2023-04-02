var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApiDb>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));

builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApiDb>();
    db.Database.EnsureCreated();
}

app.MapGet("/users", async (IUserRepository repo) => Results
        .Ok(await repo.GetUsersAsync()))
    .Produces<List<User>>(StatusCodes.Status200OK)
    .WithName("AllUsers")
    .WithTags("Getters");

app.MapGet("/users/{Id}", async (int id, IUserRepository repo) => Results
        .Ok(await repo.GetUserAsync(id)) is { } user
        ? Results.Ok(user)
        : Results.NotFound())
    .Produces<User>()
    .WithName("User")
    .WithTags("Getters");

app.MapGet("/users/search/name/{query}",
        async (string query, IUserRepository repository) =>
            await repository.GetUsersAsync(query) is {} users && users.Any()
                ? Results.Ok(users)
                : Results.NotFound(Array.Empty<User>()))
    .Produces<List<User>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("SearchUsers")
    .WithTags("Getters");
    //.ExcludeFromDescription();

app.MapPost("/users", async ([FromBody] User user, IUserRepository repo) =>
{
    await repo.CreateUserAsync(user);
    await repo.SaveAsync();
    return Results.Created($"/users/{user.Id}", user);
}).Accepts<User>("application/json")
    .Produces<User>(StatusCodes.Status201Created)
    .WithName("CreateUser")
    .WithTags("Creators");

app.MapPut("/users", async ([FromBody] User user, IUserRepository repo) =>
{
    await repo.UpdateUserAsync(user);
    await repo.SaveAsync();
    return Results.NoContent();
}).Accepts<User>("application/json")
    .WithName("UpdateUser")
    .WithTags("Updaters");

app.MapDelete("/users/{Id}", async (int id, IUserRepository repo) =>
{
    await repo.DeleteUserAsync(id);
    await repo.SaveAsync();
    return Results.NoContent();
}).WithName("DeleteUser")
    .WithTags("Deleters");

// Конвейер обработки запроса - middleware 
app.UseHttpsRedirection();

app.Run();