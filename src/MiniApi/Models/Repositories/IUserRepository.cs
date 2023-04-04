namespace MiniApi.Models.Repositories;

public interface IUserRepository
{
    UserDto GetUser(User user);
}