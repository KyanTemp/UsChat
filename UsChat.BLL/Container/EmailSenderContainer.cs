using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace UsChat.BLL.Container
{
    public class EmailSenderContainer
    {
        private readonly string smtpServer = "smtp.gmail.com";
        private readonly int port = 587;
        private readonly string fromEmail = "kyantemptest@gmail.com";
        private readonly string password = "fqqi kjiw znnw zywv";

        public void SendEmail(string to, string subject, string body)
        {
            using (var client = new SmtpClient(smtpServer, port))
            {
                client.Credentials = new NetworkCredential(fromEmail, password);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(to);

                try
                {
                    client.Send(mailMessage);
                    Console.WriteLine("Email sent successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send email: {ex.Message}");
                }
            }
        }
    }
}
