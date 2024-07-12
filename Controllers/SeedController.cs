using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RoomFInder.Models;

namespace RoomFInder.Controllers;

public class SeedController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public SeedController(UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }
    // GET
    public async Task<IActionResult> Index()
    {
        await SeedRole();
        if (! _userManager.Users.Any())
        {
            var user = new ApplicationUser
            {
                UserName = "karkialson321@gmail.com",
                Email = "karkialson321@gmail.com",
                FirstName = "test",
                LastName = "test",
                IsRoomOwner = true,
                EmailConfirmed = true
            };

              await _userManager.CreateAsync(user, "Admin@123");
              await _userManager.AddToRoleAsync(user, "Admin");
        }

        return Ok("ok") ;
    }

    public async Task SeedRole()
    {
        if (!_roleManager.Roles.Any())
        {
            var roles =
                new IdentityRole
                {
                    Name = "Admin"

                };
            var rolesSecond =
                new IdentityRole
                {
                    Name = "User"

                };
            await _roleManager.CreateAsync(roles);
            await _roleManager.CreateAsync(rolesSecond);
        }
    }
    
}