namespace MiniApi.Models;

public class UserRepository : IUserRepository
{
    private readonly UserDto[] _userDto =
    {
        new("Вася", "123"),
        new("Маша", "123"),
        new("Петя", "123"),
    };
    public UserDto GetUser(User user) => new(user.Name, user.Password);
}