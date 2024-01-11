using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Microsoft.VisualBasic;
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
    options.DefaultScheme = "MultiAuthSchemes";
    options.DefaultChallengeScheme = "MultiAuthSchemes";
}).AddJwtBearer("ApiAuthenticationScheme", options =>
{
    options.TokenValidationParameters = Constants.TokenValidationParameters(
        builder.Configuration, "Jwt:ApiKey");
}).AddJwtBearer("RoomAuthenticationScheme", options =>
{
    options.TokenValidationParameters = Constants.TokenValidationParameters(
        builder.Configuration, "Jwt:RoomKey");
}).AddPolicyScheme("MultiAuthSchemes", JwtBearerDefaults.AuthenticationScheme, options =>
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
                ? "ApiAuthenticationScheme"
                : "RoomAuthenticationScheme";
        }

        return "RoomAuthenticationScheme";
    };
});

builder.Services.AddAuthorization(options =>
{
    var apiAuthenticationScheme = new AuthorizationPolicyBuilder("ApiAuthenticationScheme");
    options.AddPolicy("ApiAuthenticationScheme", apiAuthenticationScheme
        .RequireAuthenticatedUser()
        .Build());
    var RoomAuthenticationScheme = new AuthorizationPolicyBuilder("RoomAuthenticationScheme");
    options.AddPolicy("RoomAuthenticationScheme", RoomAuthenticationScheme
        .RequireAuthenticatedUser()
        .Build());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();
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