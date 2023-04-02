var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var users = new List<User>();

app.MapGet("/users", () => users);
app.MapGet("/users/{Id}", (int id) => users.FirstOrDefault(u => u.Id == id));

app.MapPost("/users", (User user) => users.Add(user));

app.MapPut("/users", (User user) =>
{
    var index = users.FindIndex(u => u.Id == user.Id);
    if (index < 0) throw new Exception("Not found");
    users[index] = user;
});

app.MapDelete("/users/{Id}", (int id) =>
{
    var index = users.FindIndex(u => u.Id == id);
    if (index < 0) throw new Exception("Not found");
    users.RemoveAt(index);
});

app.Run();



public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public double Ratio { get; set; }
}