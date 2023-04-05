using MiniApi.Models.Entities.Events;

var builder = WebApplication.CreateBuilder(args);

RegistryServices(builder.Services);

var app = builder.Build();

Configure(app);

//app.MapGet("/events/search/numbers/{query}",
//        async (NumberInfo query, IEventRepository repository) =>
//            await repository.GetEventsAsync(query) is { } events && events.Any()
//                ? Results.Ok(events)
//                : Results.NotFound(Array.Empty<User>()))
//    .Produces<List<User>>()
//    .Produces(StatusCodes.Status404NotFound)
//    .ExcludeFromDescription();

foreach (var item in app.Services.GetServices<IApi>()) item.Register(app);

app.Run();

void RegistryServices(IServiceCollection services)
{
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();

    services.AddDbContext<ApiDb>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));

    services.AddScoped<IEventRepository, EventRepository>();
    services.AddSingleton<ITokenService, TokenService>();
    services.AddSingleton<IUserRepository, UserRepository>();
    services.AddAuthorization();
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
            };
        });

    services.AddTransient<IApi, EventApi>();
    services.AddTransient<IApi, AuthApi>();
}

void Configure(WebApplication application)
{
    application.UseAuthentication();
    application.UseAuthorization();

    if (application.Environment.IsDevelopment())
    {
        application.UseSwagger();
        application.UseSwaggerUI();
        using var scope = application.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApiDb>();
        db.Database.EnsureCreated();
    }

    application.UseHttpsRedirection();
}