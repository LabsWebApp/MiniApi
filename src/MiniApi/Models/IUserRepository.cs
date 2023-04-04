namespace MiniApi.Models;

public interface IUserRepository
{
    UserDto GetUser(User user);
}