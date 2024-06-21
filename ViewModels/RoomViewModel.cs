using System.ComponentModel.DataAnnotations;

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

        [Required]
        [Url]
        public string ImageUrl { get; set; }
    }
}