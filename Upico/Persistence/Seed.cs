using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upico.Core.Domain;
using Upico.Core.StaticValues;

namespace Upico.Persistence
{
    public class Seed
    {
        public static async Task SeedData(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // this seed method for creating some sample data in database

            //initalizing some roles
            var roleAdmin = new IdentityRole(RoleNames.RoleAdmin);
            var roleUser = new IdentityRole(RoleNames.RoleUser);

            await roleManager.CreateAsync(roleAdmin);
            await roleManager.CreateAsync(roleUser);


            //initializing some users
            var users = new List<AppUser>
            {
                new AppUser
                {
                    DisplayName = "Vinh",
                    UserName = "vinh",
                    Email = "vinh@gmail.com"
                },
                new AppUser
                {
                    DisplayName = "Tam",
                    UserName = "tam",
                    Email = "tiger@test.com"
                },
                new AppUser
                {
                    DisplayName = "HuyBoDo",
                    UserName = "huy",
                    Email = "huy@test.com"
                },
                new AppUser
                {
                    DisplayName = "Nghia",
                    UserName = "nghiax",
                    Email = "nghia@test.com"
                },
            };

            foreach (var user in users)
            {
                //Creating user
                await userManager.CreateAsync(user, "User11");

                //Registering user to role: User
                await userManager.AddToRoleAsync(user, roleUser.Name);
            }


            //Creating and Regitering user admin to role: Admin
            var userAdmin = new AppUser
            {
                DisplayName = "Admin",
                UserName = "admin",
                Email = "admin@gmail.com",

            };
            await userManager.CreateAsync(userAdmin, "Admin1");
            await userManager.AddToRoleAsync(userAdmin, roleAdmin.Name);
        }
    }
}
