using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;

using System.Threading.Tasks;

namespace WingedKeys.Services
{
    public class EmailService
    {

        private IConfiguration Configuration { get; }

        public EmailService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public Task SendEmailAsync(string emailAddress, string subject, string htmlContent)
        {
            return Execute(emailAddress, subject, htmlContent);
        }

        public Task Execute(string emailAddress, string subject, string htmlContent)
        {
            var client = new SendGridClient(Configuration.GetValue<string>("SendGridApiKey"));
            var from = new EmailAddress(Configuration.GetValue<string>("SendGridEmail"), "Connecticut Office of Early Childhood");
            var to = new EmailAddress(emailAddress);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);

            return client.SendEmailAsync(msg);
        }
    }
}
