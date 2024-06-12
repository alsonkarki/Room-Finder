using Microsoft.AspNetCore.Identity;

namespace RoomFInder.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public bool IsRoomOwner { get; set; }
    public ICollection<Room> Rooms { get; set; }
}