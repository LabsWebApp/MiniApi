var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApiDb>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));

builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApiDb>();
    db.Database.EnsureCreated();
}

app.MapGet("/users", async (IUserRepository repo) => Results.Ok(await repo.GetUsersAsync()));
app.MapGet("/users/{Id}", async (int id, IUserRepository repo) =>
    Results.Ok(await repo.GetUserAsync(id)) is { } user
        ? Results.Ok(user)
        : Results.NotFound());

app.MapPost("/users", async (User user, IUserRepository repo) =>
{
    await repo.CreateUserAsync(user);
    await repo.SaveAsync();
    return Results.Created($"/users/{user.Id}", user);
});

app.MapPut("/users", async ([FromBody] User user, IUserRepository repo) =>
{
    await repo.UpdateUserAsync(user);
    await repo.SaveAsync();
    return Results.NoContent();
});

app.MapDelete("/users/{Id}", async (int id, IUserRepository repo) =>
{
    await repo.DeleteUserAsync(id);
    await repo.SaveAsync();
    return Results.NoContent();
});

// Конвейер обработки запроса - middleware 
app.UseHttpsRedirection();

app.Run();