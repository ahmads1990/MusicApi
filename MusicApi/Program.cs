using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MusicApi.Helpers.Config;
using MusicApi.Helpers.Config.FilesConfig;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// File config
builder.Services.Configure<TrackFileConfig>(builder.Configuration.GetSection("FileConfig:TrackFileConfig"));
builder.Services.Configure<AlbumImageFileConfig>(builder.Configuration.GetSection("FileConfig:AlbumImageFileConfig"));
builder.Services.Configure<UserImageFileConfig>(builder.Configuration.GetSection("FileConfig:UserImageFileConfig"));

// Mapster
// Tell Mapster to scan this assambly searching for the Mapster.IRegister classes and execute them
TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// Security
// configure jwt helper class to use jwt config info
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("Jwt"));
// add Identity with options configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
}).AddEntityFrameworkStores<AppDbContext>();

// Add Authentication with jwt config
builder.Services.AddAuthentication(options =>{
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.SaveToken = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        };
    });

builder.Services.AddAuthorization();

// Routing Config
builder.Services.AddRouting(options => options.LowercaseUrls = true);

// DI
builder.Services.AddScoped<ITrackRepo, TrackRepo>();
builder.Services.AddScoped<IGenreRepo, GenreRepo>();

builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Use swagger in development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//app.MapIdentityApi<IdentityUser>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
