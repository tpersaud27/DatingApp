using DatingApp.DAL;
using DatingApp.Services.Extensions;
using DatingApp.Services.Implementation;
using DatingApp.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;


// This creates the web application instance
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Adding services from extension method
builder.Services.AddApplicationServices(builder.Configuration);

// Adding CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", b => b.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
});

// Authentication Service
builder.Services.AddIdentityServices(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
// When a HTTP request comes in the pipeline we can do something with it using middleware

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

app.Run();
