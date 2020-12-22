using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;

using System.Threading.Tasks;

namespace WingedKeys.Services
{
    public class EmailService
    {

        public EmailService()
        {
        }

        public Task SendEmailAsync(string emailAddress, string subject, string htmlContent)
        {
            return Execute(emailAddress, subject, htmlContent);
        }

        public Task Execute(string emailAddress, string subject, string htmlContent)
        {
            var client = new SendGridClient(Startup.Configuration.GetValue<string>("SendGridApiKey"));
            var from = new EmailAddress(Startup.Configuration.GetValue<string>("SendGridEmail"), "OEC ECE Reporter");
            var to = new EmailAddress(emailAddress);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);

            return client.SendEmailAsync(msg);
        }
    }
}
