using DatingApp.API.Entities;
using DatingApp.API.SignalR;
using DatingApp.DAL;
using DatingApp.DAL.UserSeedData;
using DatingApp.Domain.Entities;
using DatingApp.Services.Extensions;
using DatingApp.Services.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


// This creates the web application instance
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Adding services from extension method
builder.Services.AddApplicationServices(builder.Configuration);

// Adding CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", b => b.AllowAnyHeader()
                                        .AllowAnyMethod()
                                        .AllowCredentials()
                                        .WithOrigins("https://localhost:4200"));
});

// Authentication Service
builder.Services.AddIdentityServices(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();



// Configure the HTTP request pipeline.
// When a HTTP request comes in the pipeline we can do something with it using middleware

// Exception handling middleware should be at the top of the pipeline
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Using CORS Policy
app.UseCors("AllowAll");

// The app will always run on HTTPS
//app.UseHttpsRedirection();

// This checks if the user has a valid token
app.UseAuthentication();
// This decides what the user is allowed to do
app.UseAuthorization();

app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();

    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();

    // This will create a database with the seed data
    await context.Database.MigrateAsync();
    // This will remove all of the connections from the database
    await context.Database.ExecuteSqlRawAsync("DELETE FROM [Connections]");
    await Seed.SeedUsers(userManager, roleManager);
}
catch (Exception ex)
{
    var logger = services.GetService<ILogger<Program>>();
    logger.LogError(ex, "An error occured during migration");
}

app.Run();
