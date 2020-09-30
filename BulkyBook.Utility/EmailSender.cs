using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailOptions emailOptions;

        // Ovo se može napravit uvijek kad treba ucitat nesto iz appsettings.json => 
        //napravit klasu (EmailOptions) sa istim nazivima kao u i appsettings i pomocu dependency-a ucitat podatke iz IOptions<T>
        public EmailSender(IOptions<EmailOptions> options)
        {
            emailOptions = options.Value;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(emailOptions.SendGridKey, subject, htmlMessage, email);
        }

        private Task Execute(string sendGridKey, string subject, string message, string email)
        {
            // var apiKey = Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY"); --> Vec imamo
            var client = new SendGridClient(sendGridKey);
            var from = new EmailAddress("admin@bulky.com", "Bulky Books");
            // var subject = "Sending with SendGrid is Fun"; --> Dobivamo u parametrima
            var to = new EmailAddress(email, "End User");
            //var plainTextContent = "and easy to do anywhere, even with C#";
            //var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", message);
            
            return client.SendEmailAsync(msg);
        }
    }
}
