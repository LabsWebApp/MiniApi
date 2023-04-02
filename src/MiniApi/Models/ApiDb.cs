namespace MiniApi.Models;

public class ApiDb : DbContext
{
    public DbSet<User> Users => Set<User>();

    public ApiDb(DbContextOptions<ApiDb> options) : base(options){}
}