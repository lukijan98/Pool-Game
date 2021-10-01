using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
namespace pool_game_web.Data
{
    public class DbInitializer
    {

        public async static Task Initialize(ApplicationDbContext context,UserManager<IdentityUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            // In Startup I am creating first Admin Role and creating a default Admin User 
            var role = await roleManager.RoleExistsAsync("Admin");
            if (!role)  
            {  
    
                // first we create Admin role   
                IdentityRole identityRoleAdmin = new IdentityRole
                {
                    Name = "Admin"
                };
                // Saves the role in the underlying AspNetRoles table
                IdentityResult identityResultAdmin = await roleManager.CreateAsync(identityRoleAdmin);


                IdentityRole identityRoleUser = new IdentityRole
                {
                    Name = "User"
                };
                IdentityResult identityResultUser = await roleManager.CreateAsync(identityRoleUser);


                // Copy data from RegisterViewModel to IdentityUser
                var user = new IdentityUser
                {
                    UserName = "admin@email.com",
                    Email = "admin@email.com"
                };

                // Store user data in AspNetUsers database table
                var userResult = await userManager.CreateAsync(user, "Admin1!");

                if (identityResultAdmin.Succeeded&&identityResultUser.Succeeded&&userResult.Succeeded)  
                {  
                    var result = await userManager.AddToRoleAsync(user, "Admin");  
    
                }
                else
                {
                    Console.WriteLine("NOOOOOOOOOOO");
                }  
            }  
        }
    }
}