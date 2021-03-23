using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace APP.UTILS
{
    public static class SendEmail
    {
        public static bool SendEmailTo(string mail_to, string mail_subject, string mail_body)
        {
            bool result = false;
            try
            {
                SmtpClient client = new SmtpClient("smtp.gmail.com");
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("dangdat2507@gmail.com", "datdatdatdat");
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("dangdat2507@gmail.com");
                mailMessage.To.Add(mail_to);
                mailMessage.Body = mail_body;
                mailMessage.Subject = mail_subject;
                client.Send(mailMessage);
                result = true;
            }
            catch (Exception ex) { result = false; }
            return result;
        }
    }
}
