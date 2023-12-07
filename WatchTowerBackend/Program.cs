using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using WatchTowerAPI.DataAccess.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using WatchTowerAPI.BusinessLogical.Repositories.CameraRepository;
using WatchTowerAPI.BusinessLogical.Repositories.RoomRepository;
using WatchTowerAPI.BusinessLogical.Repositories.UserRepository;
using WatchTowerBackend.Presentation.SwaggerConfiguration;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<WatchTowerDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("LocalJW")));
builder.Services.AddControllers();
builder.Services.AddTransient<IRoomRepository, RoomRepository>();
builder.Services.AddTransient<ICameraRepository, CameraRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
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
}).AddJwtBearer("ApiAuthenticationScheme", options => {
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:ApiKey"]))
    };
}).AddJwtBearer("RoomAuthenticationScheme", options => {
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:RoomKey"]))
    };
}).AddPolicyScheme("MultiAuthSchemes",JwtBearerDefaults.AuthenticationScheme, options =>
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
                ? "ApiAuthenticationScheme" : "RoomAuthenticationScheme";
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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WatchTowerDbContext>();
    db.Database.Migrate();
}

app.Run();