namespace RoomFInder.Services;

public class AuthMessageSenderOptions
{
    public string MailServer { get; set; }
    public int MailPort { get; set; }
    public string SenderName { get; set; }
    public string SenderEmail { get; set; }
    public string SenderPassword { get; set; }
}