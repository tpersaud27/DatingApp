using DatingApp.DAL;
using DatingApp.Services.Implementation;
using DatingApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


            // JWT Service
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }


    }
}
