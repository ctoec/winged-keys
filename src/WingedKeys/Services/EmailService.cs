using SendGrid;
using SendGrid.Helpers.Mail;

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
            //  TODO: PULL THIS API KEY OUTTA HERE, AND THEN CHANGE IT
            var client = new SendGridClient("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");
            var from = new EmailAddress("no-reply@ctoecskylight.com", "Connecticut Office of Early Childhood");
            var to = new EmailAddress(emailAddress);


            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);

            return client.SendEmailAsync(msg);
        }
    }
}
