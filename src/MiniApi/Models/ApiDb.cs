namespace MiniApi.Models;

public class ApiDb : DbContext
{
    public DbSet<Event> Events => Set<Event>();

    public ApiDb(DbContextOptions<ApiDb> options) : base(options){}
}