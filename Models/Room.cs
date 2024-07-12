using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoomFInder.Models;

public class Room
{
    [Key]
    public int RoomId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be a positive number.")]
    public int Capacity { get; set; }

    [Required]
    [Range(0.0, Double.MaxValue, ErrorMessage = "Price must be a non-negative number.")]
    public decimal Price { get; set; }

    [Required]
    public bool IsAvailable { get; set; }

    [StringLength(500)]
    public string Description { get; set; }

    [Required]
    public string Location { get; set; }

    [Required]
    public string ImageUrl { get; set; }
    
    [Required]
    public string RoomOwnerId { get; set; }
        
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; } = DateTime.Now;

    [ForeignKey(nameof(RoomOwnerId))]
    public virtual ApplicationUser Owner { get; set; }
    //public ICollection<RoomAmenity> RoomAmenities { get; set; }
}