using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using MobileMonitoringBackend.BusinessLogical.Repositories.CameraRepository;
using MobileMonitoringBackend.BusinessLogical.Repositories.RecordingRepository;
using MobileMonitoringBackend.BusinessLogical.Repositories.RoomRepository;
using MobileMonitoringBackend.BusinessLogical.Repositories.UserRepository;
using MobileMonitoringBackend.BusinessLogical.Repositories.VideoServerRepository;
using MobileMonitoringBackend.BusinessLogical.Services;
using MobileMonitoringBackend.DataAccess.DbContexts;
using MobileMonitoringBackend.Presentation.SwaggerConfiguration;
using Constants = MobileMonitoringBackend.BusinessLogical.Utils.Constants;


var builder = WebApplication.CreateBuilder(args);

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
builder.Configuration
    .AddJsonFile($"appsettings.json", true, true)
    .AddJsonFile($"appsettings.{environment}.json", true, true)
    .AddEnvironmentVariables();

builder.Services.AddDbContext<MobileMonitoringDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddControllers();
builder.Services.AddTransient<IRoomRepository, RoomRepository>();
builder.Services.AddTransient<ICameraRepository, CameraRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IRecordingRepository, RecordingRepository>();
builder.Services.AddTransient<IVideoServerRepository, VideoServerRepository>();
builder.Services.AddSingleton<RecordingCamerasCache>();
builder.Services.AddHttpClient<IVideoServerRepository, VideoServerRepository>(client =>
{
    client.BaseAddress = new(Constants.RecordBaseUrl);
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.OperationFilter<AuthorizationOperationFilter>();
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = Constants.MultiAuthSchemes;
    options.DefaultChallengeScheme = Constants.MultiAuthSchemes;
}).AddJwtBearer(Constants.ApiAuthScheme, options =>
{
    options.TokenValidationParameters = Constants.TokenValidationParameters(
        builder.Configuration, "Jwt:ApiKey");
}).AddJwtBearer(Constants.RoomAuthScheme, options =>
{
    options.TokenValidationParameters = Constants.TokenValidationParameters(
        builder.Configuration, "Jwt:RoomKey");
}).AddPolicyScheme(Constants.MultiAuthSchemes, JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.ForwardDefaultSelector = context =>
    {
        string authorization = context.Request.Headers[HeaderNames.Authorization];
        if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
        {
            var token = authorization.Substring("Bearer ".Length).Trim();
            var jwtHandler = new JwtSecurityTokenHandler();
            var rawToken = jwtHandler.ReadJwtToken(token);
            return (rawToken.Claims.SingleOrDefault(c => c.Value == "TokenAPI") is not null)
                ? Constants.ApiAuthScheme
                : Constants.RoomAuthScheme;
        }

        return Constants.RoomAuthScheme;
    };
});

builder.Services.AddAuthorization(options =>
{
    var apiAuthenticationScheme = new AuthorizationPolicyBuilder(Constants.ApiAuthScheme);
    options.AddPolicy(Constants.ApiAuthScheme, apiAuthenticationScheme
        .RequireAuthenticatedUser()
        .Build());
    var roomAuthenticationScheme = new AuthorizationPolicyBuilder(Constants.RoomAuthScheme);
    options.AddPolicy(Constants.RoomAuthScheme, roomAuthenticationScheme
        .RequireAuthenticatedUser()
        .Build());
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(corsPolicyBuilder =>
    corsPolicyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
while (true)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MobileMonitoringDbContext>();
        db.Database.Migrate();
        break;
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
}

app.Run();