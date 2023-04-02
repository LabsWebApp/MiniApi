var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApiDb>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApiDb>();
    db.Database.EnsureCreated();
}

app.MapGet("/users", async (ApiDb db) => await db.Users.ToListAsync());
app.MapGet("/users/{Id}", async (int id, ApiDb db) =>
    await db.Users.FirstOrDefaultAsync(u => u.Id == id) is { } user
        ? Results.Ok(user)
        : Results.NotFound());

//app.MapPost("/users", 
//    async ([FromBody]User user, /*[FromServices]*/ApiDb db, HttpResponse response) =>
//{
//    db.Users.Add(user);
//    await db.SaveChangesAsync();
//    response.StatusCode = 201;
//    response.Headers.Location = $"/users/{user.Id}";
//});

app.MapPost("/users", async ([FromBody] User user, ApiDb db) =>
    {
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return Results.Created($"/users/{user.Id}", user);
    });

app.MapPut("/users", async ([FromBody] User user, ApiDb db) =>
{
    var oldUser = await db.Users.FindAsync(user.Id);
    if (oldUser == null) return Results.NotFound();
    if (oldUser.Name != user.Name) oldUser.Name = user.Name;
    if (oldUser.Age != user.Age) oldUser.Age = user.Age;
    if (Math.Abs(oldUser.Ratio - user.Ratio) > 0.0001) oldUser.Ratio = user.Ratio;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/users/{Id}", async (int id, ApiDb db) =>
{
    var oldUser = await db.Users.FindAsync(id);
    if (oldUser == null) return Results.NotFound();
    db.Users.Remove(oldUser);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Конвейер обработки запроса - middleware 
app.UseHttpsRedirection();

app.Run();


public class ApiDb : DbContext
{
    public DbSet<User> Users => Set<User>();

    public ApiDb(DbContextOptions<ApiDb> options) : base(options){}
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public double Ratio { get; set; }
}