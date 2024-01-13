using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Mapster
// Tell Mapster to scan this assambly searching for the Mapster.IRegister classes and execute them
TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// DI
builder.Services.AddScoped<ITrackRepo, TrackRepo>();
builder.Services.AddScoped<IGenreRepo, GenreRepo>();

var app = builder.Build();

// Use swagger in development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
