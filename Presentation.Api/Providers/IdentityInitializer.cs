using Data.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace Presentation.Api.Providers
{
    /// <summary>
    ///
    /// </summary>
    public static class IdentityDataInitializer
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="roleManager"></param>
        public static void SeedData(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="userManager"></param>
        public static void SeedUsers(UserManager<User> userManager)
        {
            if (userManager.FindByNameAsync
("admin@ovidos.com").Result == null)
            {
                User user = new User();
                user.UserName = "admin@ovidos.com";
                user.Email = "admin@ovidos.com";
                user.FirstName = "John";
                user.LastName = "Doe";
                user.PhoneNumber = "+901234567890";

                var task = userManager.CreateAsync(user, "123456");
                task.Wait();

                if (task.Result.Succeeded)
                {
                    var response = userManager.AddToRoleAsync(user, "admin").Result;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="roleManager"></param>
        public static void SeedRoles(RoleManager<Role> roleManager)
        {
            if (!roleManager.RoleExistsAsync("admin").Result)
            {
                var admin = new Role()
                {
                    Name = "admin",
                    NormalizedName = "admin",
                    DisplayName = "Yönetici"
                };

                var adminResult = roleManager.CreateAsync(admin).Result;
            }

            if (!roleManager.RoleExistsAsync("member").Result)
            {
                var member = new Role()
                {
                    Name = "member",
                    NormalizedName = "member",
                    DisplayName = "Üye"
                };

                var memberResult = roleManager.CreateAsync(member).Result;
            }
        }
    }
}