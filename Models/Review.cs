namespace RoomFInder.Models;

public class Review
{
    public int ReviewId { get; set; }
    public int RoomId { get; set; }  // Foreign key to the Room
    public string UserName { get; set; }
    public string Content { get; set; }
    public int Rating { get; set; }  // Rating out of 5
    public DateTime CreatedAt { get; set; }

    // Navigation property
    public Room Room { get; set; }
}