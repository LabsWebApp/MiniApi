namespace MiniApi.Models.EF.Repositories;

public class UserRepository : IUserRepository
{
    private static IEnumerable<UserDto> Users => new UserDto[]
    {
        new ("Vasja", "123"),
        new ("Петя", "123"),
        new ("Маша", "123")
    };

    public UserDto GetUser(User user) =>
        Users.FirstOrDefault(u =>
            string.Equals(u.Name, user.Name) &&
            string.Equals(u.Password, user.Password)) ??
        throw new Exception();
}