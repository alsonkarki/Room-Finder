using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace RoomFInder.Services;

public class EmailSender : IEmailSender
{
    private readonly AuthMessageSenderOptions _options;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor, ILogger<EmailSender> logger)
    {
        _options = optionsAccessor.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_options.SenderName, _options.SenderEmail));
        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = htmlMessage
        };

        message.Body = bodyBuilder.ToMessageBody();

        try
        {
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_options.MailServer, _options.MailPort, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_options.SenderEmail, _options.SenderPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            _logger.LogInformation($"Email sent to {email}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sending email: {ex.Message}");
        }
    }
}


