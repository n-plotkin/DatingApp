using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            if (await userManager.Users.AnyAsync())
            {
                return;
            }

            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);


            var roles = new List<AppRole>
            {
                new AppRole{Name = "Member"},
                new AppRole{Name = "Admin"},
                new AppRole{Name = "Moderator"},
                new AppRole{Name = "Dummy1"},
                new AppRole{Name = "Dummy2"},
                new AppRole{Name = "Dummy3"},
                new AppRole{Name = "Dummy4"},
                new AppRole{Name = "Dummy5"},
                new AppRole{Name = "Dummy6"},
                new AppRole{Name = "Dummy7"},
                new AppRole{Name = "Dummy8"},
                new AppRole{Name = "Dummy9"},
                new AppRole{Name = "Dummy10"},
                new AppRole{Name = "Dummy11"},


            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }





            foreach (var user in users)
            {
                foreach (var prop in user.GetType().GetProperties())
                {
                    Console.WriteLine($"{prop.Name}: {prop.GetValue(user)}");
                }
                Console.WriteLine(); // Add an empty line for better readability between users
            }

            foreach (var user in users)
            {
                user.UserName = user.UserName.ToLower();

                //no need to save, this method will create and save
                await userManager.CreateAsync(user, "Pa$$w0rd");

                Console.Write("LOOK HERE: " + user + " " + user.Id);

            }
            foreach (var user in users)
            {
                if (await roleManager.RoleExistsAsync("Member")) await userManager.AddToRoleAsync(user, "Member");

                Console.Write("LOOK HERE PART 2: " + user + " " + user.Id);

            }

            var admin = new AppUser
            {
                UserName = "admin"
            };


            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });

        }
        public static async Task TrimTempRoles(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            var roles = new List<AppRole>
            {
                new AppRole{Name = "Dummy1"},
                new AppRole{Name = "Dummy2"},
                new AppRole{Name = "Dummy3"},
                new AppRole{Name = "Dummy4"},
                new AppRole{Name = "Dummy5"},
                new AppRole{Name = "Dummy6"},
                new AppRole{Name = "Dummy7"},
                new AppRole{Name = "Dummy8"},
                new AppRole{Name = "Dummy9"},
                new AppRole{Name = "Dummy10"},
                new AppRole{Name = "Dummy11"},
            };

            foreach (var role in roles)
            {
                await roleManager.DeleteAsync(role);
            }
        }
    }
}