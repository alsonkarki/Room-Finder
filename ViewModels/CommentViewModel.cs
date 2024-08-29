using RoomFInder.Models;

namespace RoomFInder.ViewModels;

public class CommentViewModel
{
    public int CommentId { get; set; }
    public string Text { get; set; }
    public string UserName { get; set; }
    public string Content { get; set; }
    public int Likes { get; set; }
    public DateTime CreatedAt { get; set; }
    public int RoomId { get; set; }


    public List<Comment> Comments { get; set; } = new List<Comment>();
}