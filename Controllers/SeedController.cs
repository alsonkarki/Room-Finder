using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RoomFInder.Models;

namespace RoomFInder.Controllers;

public class SeedController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public SeedController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }
    // GET
//     public async Task<IActionResult> Index()
//     {
//         await SeedRole();
//         if (! _userManager.Users.Any())
//         {
//             var user = new ApplicationUser
//             {
//                 UserName = "karkialson321@gmail.com",
//                 Email = "karkialson321@gmail.com",
//                 FirstName = "test",
//                 LastName = "test",
//                 IsRoomOwner = true,
//                 EmailConfirmed = true
//             };
//
//               await _userManager.CreateAsync(user, "Admin@123");
//               await _userManager.AddToRoleAsync(user, "Admin");
//         }
//
//         return Ok("ok") ;
//     }
//
//     public async Task SeedRole()
//     {
//         if (!_roleManager.Roles.Any())
//         {
//             var roles =
//                 new IdentityRole
//                 {
//                     Name = "Admin"
//
//                 };
//             var rolesSecond =
//                 new IdentityRole
//                 {
//                     Name = "User"
//
//                 };
//             await _roleManager.CreateAsync(roles);
//             await _roleManager.CreateAsync(rolesSecond);
//         }
//     }
//     
// }

    public async Task<IActionResult> Index()
    {
        // Seed roles if they don't exist
        await SeedRole();

        // Check if any users exist in the database
        if (!_userManager.Users.Any())
        {
            // Create a new user
            var user = new ApplicationUser
            {
                UserName = "karkialson321@gmail.com",
                Email = "karkialson321@gmail.com",
                FirstName = "test",
                LastName = "test",
                IsRoomOwner = true,
                EmailConfirmed = true
            };

            // Attempt to create the user
            var result = await _userManager.CreateAsync(user, "Admin@123");

            if (result.Succeeded)
            {
                // Add the user to the Admin role if creation is successful
                await _userManager.AddToRoleAsync(user, "Admin");
            }
            else
            {
                // Handle errors in user creation
                foreach (var error in result.Errors)
                {
                    // Log the errors (you could use a logger instead of ModelState)
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return BadRequest(ModelState);
            }
        }

        return Ok("User creation and role assignment successful");
    }

    public async Task SeedRole()
    {
        if (!_roleManager.Roles.Any())
        {
            // List of roles to seed
            var roles = new List<IdentityRole>
            {
                new IdentityRole { Name = "Admin" },
                new IdentityRole { Name = "RoomOwner" },
                new IdentityRole { Name = "User" }
            };

            // Create each role if it doesn't exist
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role.Name))
                {
                    await _roleManager.CreateAsync(role);
                }
            }
        }
    }
}
    
