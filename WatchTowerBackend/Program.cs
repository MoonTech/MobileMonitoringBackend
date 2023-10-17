using WatchTowerAPI.DataAccess.DbContexts;
using Microsoft.EntityFrameworkCore;
using WatchTowerAPI.BusinessLogical.Repositories.CameraRepository;
using WatchTowerAPI.BusinessLogical.Repositories.RoomRepository;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<WatchTowerDbContext>(
    options => options.UseSqlServer(builder.Configuration["ConnectionString"]));


builder.Services.AddControllers();
builder.Services.AddTransient<IRoomRepository, RoomRepository>();
builder.Services.AddTransient<ICameraRepository, CameraRepository>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();