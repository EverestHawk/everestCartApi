using ApplicationCore.Interfaces;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            //Wire this up to actual email sending logic via local SMTP, SendGrid etc.
            return Task.CompletedTask;
        }
    }
}
