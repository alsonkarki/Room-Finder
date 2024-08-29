namespace RoomFInder.Models;

public class Like
{
    public int LikeId { get; set; }
    public int CommentId { get; set; }  // Foreign key to the Comment
    public string UserName { get; set; }

    // Navigation property
    public Comment Comment { get; set; }
}