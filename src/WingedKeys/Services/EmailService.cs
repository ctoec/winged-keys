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

        public Task SendEmailAsync()
        {
            return Execute();
        }

        public Task Execute()
        {
            //  TODO: PULL THIS API KEY OUTTA HERE, AND THEN CHANGE IT
            var client = new SendGridClient("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");
            var from = new EmailAddress("no-reply@ctoecskylight.com");
            var subject = "JUST A TEST";
            var to = new EmailAddress("jordan@skylight.digital");
            var plainTextContent = "LOOK MORE WORDS";


            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, "");

            return client.SendEmailAsync(msg);
        }
    }
}
