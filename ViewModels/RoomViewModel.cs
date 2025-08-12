using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using RoomFInder.Models;
using RoomFInder.ViewModels;

namespace RoomFinder.ViewModels
{
    public class RoomViewModel
    {
        public int RoomId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be a positive number.")]
        public int Capacity { get; set; }

        [Required]
        [Range(0.0, double.MaxValue, ErrorMessage = "Price must be a non-negative number.")]
        public decimal Price { get; set; }

        [Required]
        public bool IsAvailable { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public string Location { get; set; }
        
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        
        [Required]
        public string PhoneNumber { get; set; }
        public string ImageUrl { get; set; } = String.Empty;
        public IFormFile ImageFile { get; set; }
        

        public int Likes { get; set; }
        public List<CommentViewModel> Comments { get; set; } = new List<CommentViewModel>();
        
        [Required]
        public PropertyType PropertyType { get; set; } 

        public IEnumerable<SelectListItem> PropertyTypes { get; set; }

    }
}