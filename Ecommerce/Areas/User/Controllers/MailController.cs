using Ecommerce.Models;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Text;
using MimeKit;
using System.Net;
using System.Net.Mail;

namespace Ecommerce.Areas.User.Controllers
{
    [Area("User")]
    public class MailController : Controller
    {
        
        [HttpGet]
        public void Index(SendMail sendMail)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(sendMail.From));
            email.To.Add(MailboxAddress.Parse(sendMail.To));
            email.Subject = sendMail.Subject;
            email.Body = new TextPart(TextFormat.Html) { Text = sendMail.Body };

            // send email
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("farmanghogha2000@gmail.com", "ugsesevrdnwgerfw");
            smtp.Send(email);
            smtp.Disconnect(true);
                        
        }
    }
}
