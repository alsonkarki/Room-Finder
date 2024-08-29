namespace RoomFInder.ViewModels;

public class ReviewViewModel
{
    public int ReviewId { get; set; }
    public int Rating { get; set; }  // e.g., 1 to 5 stars
    public string Text { get; set; }
    public string UserName { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public int RoomId { get; set; }
}