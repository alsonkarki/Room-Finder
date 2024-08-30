using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using RoomFInder.Data;
using RoomFInder.Models;
using RoomFInder.ViewModels;

namespace RoomFInder.Controllers;

    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly INotyfService _notyfService;
        private readonly AppDbContext _context;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> 
            signInManager, IEmailSender emailSender,INotyfService notyfService,AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _notyfService = notyfService;
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    IsRoomOwner = model.IsRoomOwner
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                _notyfService.Success("Sucessefully Registered");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action(
                        nameof(ConfirmEmail),
                        "Account",
                        new { userId = user.Id, code },
                        protocol: HttpContext.Request.Scheme);
                    
                    await _emailSender.SendEmailAsync(model.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.");

                    return RedirectToAction(nameof(RegisterConfirmation));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View();
        }

        [HttpGet]
        public IActionResult RegisterConfirmation()
        {
            return View();
        }
        
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        

        private bool IsValidemail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Define a regular expression for validating an email address
                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
                return emailRegex.IsMatch(email);
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        
            
    }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult PostLogin()
        {
            ViewData["RoomCount"] = _context.Rooms.Count();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (IsValidemail(model.Email))
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    var roles = await _userManager.GetRolesAsync(user);

                    _notyfService.Success("Login Successfully");

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        // Redirect back to the page where the login was initiated
                        return LocalRedirect(returnUrl);
                    }

                    // If no returnUrl is provided, check the user's role
                    if (roles.Contains("Admin"))
                    {
                        // Redirect admin users to the dashboard
                        return RedirectToAction(nameof(PostLogin));
                    }

                    // Redirect non-admin users to the home page
                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }

                if (result.IsLockedOut)
                {
                    return View("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _notyfService.Success("Logout Successfully");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }

    

