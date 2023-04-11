using System;
using System.Threading.Tasks;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace FptBook.Mail;

public class MailSettings {
    public string Mail { get; set; }
    public string DisplayName { get; set; }
    public string Password { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
}

// Send mail service
public class SendMailService : IEmailSender {
    private readonly MailSettings mailSettings;

    private readonly ILogger<SendMailService> logger;

    // mailSetting in injected
    // inject logger to log
    public SendMailService (IOptions<MailSettings> _mailSettings, ILogger<SendMailService> _logger) {
        mailSettings = _mailSettings.Value;
        logger = _logger;
        logger.LogInformation ("Create SendMailService");
    }

    public async Task SendEmailAsync (string email, string subject, string htmlMessage) {
        var message = new MimeMessage ();
        message.Sender = new MailboxAddress (mailSettings.DisplayName, mailSettings.Mail);
        message.From.Add (new MailboxAddress (mailSettings.DisplayName, mailSettings.Mail));
        message.To.Add (MailboxAddress.Parse (email));
        message.Subject = subject;

        var builder = new BodyBuilder ();
        builder.HtmlBody = htmlMessage;
        message.Body = builder.ToMessageBody ();

        // Use smpt client from mailkit
        using var smtp = new MailKit.Net.Smtp.SmtpClient ();

        try {
            smtp.Connect (mailSettings.Host, mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate (mailSettings.Mail, mailSettings.Password);
            await smtp.SendAsync (message);
        } catch (Exception ex) {
            // if failed save the mail in SavedMails directory
            System.IO.Directory.CreateDirectory ("SavedMails");
            var emailsavefile = string.Format (@"SavedMails/{0}.eml", Guid.NewGuid ());
            await message.WriteToAsync (emailsavefile);

            logger.LogInformation ("Could not send mail, saved at - " + emailsavefile);
            logger.LogError (ex.Message);
        }

        smtp.Disconnect (true);

        logger.LogInformation ("sent mail to: " + email);

    }
}
