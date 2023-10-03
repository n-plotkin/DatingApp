using API.Data;
using Microsoft.EntityFrameworkCore;
using API.Services;
using API.Interfaces;

namespace API.Extensions
{
    //Make it static so we can use the methods inside without instantiating 
    //a new instance of the class
    public static class ApplicationServiceExtensions 
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)

        {
            services.AddDbContext<DataContext>(opt => 
            {
                opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

            //How long shall a service be added for? 
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }
    }
}