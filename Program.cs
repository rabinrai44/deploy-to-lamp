using deploy_to_linux.Core.Interfaces;
using deploy_to_linux.Helpers;
using deploy_to_linux.Infrastructure.Data;
using deploy_to_linux.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(options =>
{
    // options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version(8, 0, 27)));
    string connecStr = builder.Configuration.GetConnectionString("DefaultConnection");
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    connecStr = string.IsNullOrEmpty(databaseUrl) ? connecStr : BuildConnectionString(databaseUrl);

    //build the connection string from the environment. i.e. Heroku
    string BuildConnectionString(string databaseUrl)
    {
        var databaseUri = new Uri(databaseUrl);
        var userInfo = databaseUri.UserInfo.Split(':');
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = databaseUri.Host,
            Port = databaseUri.Port,
            Username = userInfo[0],
            Password = userInfo[1],
            Database = databaseUri.LocalPath.TrimStart('/'),
            SslMode = SslMode.Require,
            TrustServerCertificate = true
        };
        return builder.ToString();
    }
    options.UseNpgsql(connecStr);
});


builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));

// builder.WebHost.ConfigureKestrel(options =>
// {
//     options.ListenLocalhost(5000);
// });

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

var scope = app.Services.CreateScope();

var services = scope.ServiceProvider;
var loggerFactory = services.GetRequiredService<ILoggerFactory>();

try
{
    // Data migration at initial app startup
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
}
catch (Exception ex)
{
    var logger = loggerFactory.CreateLogger<Program>();
    logger.LogError(ex, "An error occurred during the migration");
}

app.Run();
