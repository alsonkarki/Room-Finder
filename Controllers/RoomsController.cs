using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomFInder.Data;
using RoomFInder.Models;
using RoomFInder.Services;
using RoomFinder.ViewModels;

namespace RoomFInder.Controllers;

 [Authorize]
    public class RoomsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ImageService _imageService;

        public RoomsController(AppDbContext context, UserManager<ApplicationUser> userManager,ImageService imageService)
        {
            _context = context;
            _userManager = userManager;
            _imageService = imageService;
        }
        
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var rooms = await _context.Rooms.Include(r => r.Owner).ToListAsync();
            return View(rooms);
        }
        

        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .Include(r => r.Owner)
                .FirstOrDefaultAsync(m => m.RoomId == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }
        
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;

                if (model.ImageFile != null)
                {
                    uniqueFileName = await _imageService.SaveImageAsync(model.ImageFile);
                }

                var room = new Room
                {
                    Name = model.Name,
                    Capacity = model.Capacity,
                    Price = model.Price,
                    IsAvailable = model.IsAvailable,
                    Description = model.Description,
                    Location = model.Location,
                    ImageUrl = uniqueFileName ?? string.Empty,
                    RoomOwnerId = _userManager.GetUserId(User)
                };

                _context.Add(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Log ModelState errors for debugging
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                System.Diagnostics.Debug.WriteLine($"ModelState Error: {error.ErrorMessage}");
            }

            return View(model);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            var model = new RoomViewModel
            {
                RoomId = room.RoomId,
                Name = room.Name,
                Capacity = room.Capacity,
                Price = room.Price,
                IsAvailable = room.IsAvailable,
                Description = room.Description,
                Location = room.Location,
                ImageUrl = room.ImageUrl
            };
            await _context.SaveChangesAsync();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RoomViewModel model)
        {
            if (id != model.RoomId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var room = await _context.Rooms.FindAsync(id);
                    room.Name = model.Name;
                    room.Capacity = model.Capacity;
                    room.Price = model.Price;
                    room.IsAvailable = model.IsAvailable;
                    room.Description = model.Description;
                    room.Location = model.Location;
                    room.ImageUrl = model.ImageUrl;

                    _context.Update(room);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(model.RoomId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .Include(r => r.Owner)
                .FirstOrDefaultAsync(m => m.RoomId == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.RoomId == id);
        }
    }
