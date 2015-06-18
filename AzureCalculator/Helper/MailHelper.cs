using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace AzureCalculator.Helper
{
    public class MailHelper
    {
        public static void Send(String subject, String recipient, String strHTML)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("paras@motifworks.com");
                mail.To.Add(recipient);
                mail.Subject = subject;

                mail.IsBodyHtml = true;

                mail.Body = strHTML;

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("paras@motifworks.com", "India!23");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}