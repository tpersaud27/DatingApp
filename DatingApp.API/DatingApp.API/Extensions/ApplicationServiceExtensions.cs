using DatingApp.DAL;
using DatingApp.DAL.AutomapperConfig;
using DatingApp.DAL.Implementation;
using DatingApp.DAL.Interfaces;
using DatingApp.Sercvices.Middleware;
using DatingApp.DAL.CloudinaryService;
using DatingApp.Services.Implementation;
using DatingApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using DatingApp.API.Implementation;
using DatingApp.API.Interfaces;

namespace DatingApp.Services.Extensions
{
    // Using static here allows us to have on instance of the class so we dont need to instantiate to use these methods
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {

            // Db Context
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
                options.UseSqlite(b =>
                {
                    b.MigrationsAssembly("DatingApp.API");
                });
            });

            services.AddCors();

            // JWT Service
            services.AddScoped<ITokenService, TokenService>();
            // Automapper Config
            services.AddAutoMapper(typeof(AutomapperConfig));

            // Repositories

            // UserRepository
            services.AddScoped<IUserRepository, UserRepository>();
            // LikesRepository
            services.AddScoped<ILikesRepository, LikesRepository>();
            // MessageRepository
            services.AddScoped<IMessageRepository, MessageRepository>();


            // Cloudinary Config
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            // Cloudinary Service
            services.AddScoped<IPhotoService, PhotoService>();
            // Updating user last active property
            services.AddScoped<LogUserActivity>();

            // SignalR
            services.AddSignalR();


            return services;
        }


    }
}
