using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ECommerce.Utility;

public class EmailSender(IConfiguration config) : IEmailSender
{
    public string SendGridSecret { get; set; } = config.GetValue<string>("SendGrid:SecretKey")!;

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // TODO: implement email sender
        // var client = new SendGridClient(SendGridSecret);
        // var from = new EmailAddress("mail@example.com", "E-Commerce");
        // var to = new EmailAddress(email);
        // var message = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);
        // await client.SendEmailAsync(message);
        return Task.CompletedTask;
    }
}