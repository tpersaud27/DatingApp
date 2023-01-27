using DatingApp.DAL;
using Microsoft.EntityFrameworkCore;


// This creates the web application instance
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});


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

// The app will always run on HTTPS
//app.UseHttpsRedirection();


app.UseAuthorization();

app.MapControllers();

app.Run();
