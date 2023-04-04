using MiniApi.Models.EF.Repositories;

namespace MiniApi.Apis;

public class AuthApi : IApi
{
    public void Register(WebApplication app) =>
        app.MapGet("/login",
            [AllowAnonymous] async (
                    HttpContext context, ITokenService tokenService, IUserRepository userRepository) =>
                await Task.Run(() =>
                {
                    User user = new()
                    {
                        Name = context.Request.Query["name"]!,
                        Password = context.Request.Query["password"]!
                    };
                    var userDto = userRepository.GetUser(user);
                    //if (userDto == null) return Results.Unauthorized();
                    var token = tokenService.BuildToken(
                        app.Configuration["Jwt:Key"]!,
                        app.Configuration["Jwt:Issuer"]!, userDto);
                    return Results.Ok(token);
                })).ExcludeFromDescription();
}