using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomFInder.Data;
using RoomFInder.Interface;
using RoomFInder.Models;
using RoomFInder.Repository;
using RoomFInder.Services;
using RoomFinder.ViewModels;
using RoomFInder.ViewModels;

namespace RoomFInder.Controllers;

 [Authorize]
    public class RoomsController : Controller
    {
        
        private readonly IGenericRepository<Room> _roomRepository;
        private readonly IGenericRepository<Comment> _commentRepository;
        private readonly IGenericRepository<Review> _reviewRepository;
        private readonly IGenericRepository<Like> _likeRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ImageService _imageService;
        private readonly INotyfService _notyfService;


        public RoomsController(IGenericRepository<Room> roomRepository,IGenericRepository<Comment> commentRepository,
            IGenericRepository<Review> reviewRepository,IGenericRepository<Like> likeRepository,
            UserManager<ApplicationUser> userManager,ImageService imageService,INotyfService notyfService)
        {
            _roomRepository = roomRepository;
            _commentRepository = commentRepository;
            _reviewRepository = reviewRepository;
            _likeRepository = likeRepository;
            _userManager = userManager;
            _imageService = imageService;
            _notyfService = notyfService;
        }
        

        public async Task<IActionResult> Index()
        {
            var rooms = await _roomRepository.GetActiveAsync();
                
            return View(rooms);
        }
        
        
        public async Task<IActionResult> Details(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            // Get the comments; ensure it’s not null
            var comments = await _commentRepository.GetCommentsByRoomIdAsync(id) ?? new List<Comment>();

            var viewModel = new RoomViewModel
            {
                RoomId = room.RoomId,
                Name = room.Name,
                Location = room.Location,
                Price = room.Price,
                IsAvailable = room.IsAvailable,
                Description = room.Description,
                PhoneNumber = room.PhoneNumber,
                ImageUrl = room.ImageUrl,
                Comments = comments.Select(c => new CommentViewModel
                {
                    CommentId = c.CommentId,
                    UserName = c.UserName,
                    Content = c.Content,
                    Likes = c.Likes?.Count() ?? 0,
                    CreatedAt = c.CreatedAt,
                    RoomId = c.RoomId
                }).ToList()
            };

            return View(viewModel);
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
                    PhoneNumber = model.PhoneNumber,
                    ImageUrl = uniqueFileName ?? string.Empty,
                    RoomOwnerId = _userManager.GetUserId(User)
                };

                await _roomRepository.AddAsync(room);
                 _notyfService.Success("Successfully Created");
                return RedirectToAction(nameof(Index));
            }
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

            var room = await _roomRepository.GetByIdAsync(id.Value);
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
            var rooms = await _roomRepository.GetByIdAsync(id);
            if (rooms == null || (rooms is IRecStatusEntity recStatusEntity && recStatusEntity.RecStatus == "D"))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var room = await _roomRepository.GetByIdAsync(id);
                if (room == null)
                {
                    return NotFound();
                }

                string uniqueFileName = room.ImageUrl;

                if (model.ImageFile != null)
                {
                    uniqueFileName = await _imageService.SaveImageAsync(model.ImageFile);

                }

                try 
                {

                room.Name = model.Name;
                room.Capacity = model.Capacity;
                room.Price = model.Price;
                room.IsAvailable = model.IsAvailable;
                room.Description = model.Description;
                room.Location = model.Location;
                room.ImageUrl = uniqueFileName;
                
                    await _roomRepository.UpdateAsync(room);
                    _notyfService.Success("Successfully Updated");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await RoomExists(room.RoomId))
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

                var room = await _roomRepository.GetByIdAsync(id.Value);
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
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            // Set RecStatus to 'D' (soft delete)
            if (room is IRecStatusEntity recStatusEntity)
            {
                recStatusEntity.RecStatus = "D";
                await _roomRepository.UpdateAsync(room); // Soft delete
                _notyfService.Success("Successfully Deleted");
            }
            else
            {
                // If not supporting RecStatus, fallback to hard delete
                await _roomRepository.DeleteAsync(room);
                _notyfService.Warning("Permanently Deleted (no RecStatus support)");
            }

            return RedirectToAction(nameof(Index));
        }
        private async Task<bool> RoomExists(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            return room != null;
        }
        
         // Comment Section
         [HttpPost]
         [ValidateAntiForgeryToken]
         public async Task<IActionResult> AddComment(CommentViewModel model)
         {
             if (ModelState.IsValid)
             {
                 var userName = _userManager.GetUserName(User);
                 if (string.IsNullOrEmpty(userName))
                 {
                     _notyfService.Error("You must be logged in to post a comment.");
                     return RedirectToAction(nameof(Details), new { id = model.RoomId });
                 }
                 var comment = new Comment
                 {
                     RoomId = model.RoomId,
                     UserName = _userManager.GetUserName(User),
                     Content = model.Content,
                     CreatedAt = DateTime.Now
                 };

                 await _commentRepository.AddAsync(comment);
                 _notyfService.Success("Comment Added Successfully");
                 return RedirectToAction(nameof(Details), new { id = model.RoomId });
             }
             else
             {
                 foreach (var modelState in ModelState.Values)
                 {
                     foreach (var error in modelState.Errors)
                     {
                         Console.WriteLine(error.ErrorMessage);
                     }
                 }
             }

             return RedirectToAction(nameof(Details), new { id = model.RoomId });
         }


        // Review Section

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(ReviewViewModel model)
        {
            if (ModelState.IsValid)
            {
                var review = new Review
                {
                    RoomId = model.RoomId,
                    UserName = _userManager.GetUserName(User),
                    Content = model.Content,
                    Rating = model.Rating,
                    CreatedAt = DateTime.Now
                };

                await _reviewRepository.AddAsync(review);
                _notyfService.Success("Review Added Successfully");
                return RedirectToAction(nameof(Details), new { id = model.RoomId });
            }

            return RedirectToAction(nameof(Details), new { id = model.RoomId });
        }

        // Like Section

        [HttpPost]
        public async Task<IActionResult> LikeComment(int commentId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment != null)
            {
                var like = new Like
                {
                    CommentId = commentId,
                    UserName = _userManager.GetUserName(User)
                };

                await _likeRepository.AddAsync(like);
                _notyfService.Success("Comment Liked Successfully");
            }

            return RedirectToAction(nameof(Details), new { id = comment.RoomId });
        }
    }
