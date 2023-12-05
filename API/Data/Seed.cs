using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using API.Helpers.Song;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace API.Data
{
    public class Seed
    {
        public static async Task ClearConnections(DataContext context)
        {
            context.Connections.RemoveRange(context.Connections);
            await context.SaveChangesAsync();
        }
        public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            Console.WriteLine("Got here");
            if (await userManager.Users.AnyAsync())
            {
                return;
            }

            var roles = new List<AppRole>
            {
                new AppRole{Name = "Member"},
                new AppRole{Name = "Admin"},
                new AppRole{Name = "Moderator"},
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }


            var admin = new AppUser
            {
                UserName = "admin"
            };


            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });

        }
    }
}