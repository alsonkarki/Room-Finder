namespace RoomFInder.Models;

public class Comment
{
    public int CommentId { get; set; }
    public int RoomId { get; set; }  // Foreign key to the Room
    public string UserName { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation property
    public Room Room { get; set; }
    public ICollection<Like> Likes { get; set; } // Likes associated with the comment
}
