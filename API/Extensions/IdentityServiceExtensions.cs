using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, 
            IConfiguration config)
        {
            services.AddIdentityCore<AppUser>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
            })
                .AddRoles<AppRole>()
                .AddRoleManager<RoleManager<AppRole>>()
                .AddEntityFrameworkStores<DataContext>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
        
                options =>
            {   
                options.TokenValidationParameters = new TokenValidationParameters 
                {
                    //Checks the key

                    ValidateIssuerSigningKey = true,
                    //Validates with our tokenkey
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding
                        .UTF8.GetBytes(config["TokenKey"])),
                    //
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                // Configures JWT Bearer events
                options.Events = new JwtBearerEvents
                {
                    // Defines a delegate for the OnMessageReceived event
                    OnMessageReceived = context => 
                    {
                        //"access_token" is the name of what signalR uses from client side when it sends up token
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        //We called it /hubs in our program class.
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;

                    }
                };


            });

            services.AddAuthorization(opt => 
            {
                opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                opt.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));
            });
            

            return services;
        }
    }
}