using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient("smtp.gmail.com ", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("ahmedkmuhamedd@gmail.com", "zfaq gsrn arye hhsh")

            };

            return client.SendMailAsync(
                new MailMessage(from: "ahmedkmuhamedd@gmail.com",
                                to: email,
                                subject,
                                htmlMessage
                                )
                { IsBodyHtml = true }
                );
        }
    }
}
