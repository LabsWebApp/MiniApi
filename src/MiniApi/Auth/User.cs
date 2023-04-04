namespace MiniApi.Auth;

public record UserDto(string Name, string Password);

public class User
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}
