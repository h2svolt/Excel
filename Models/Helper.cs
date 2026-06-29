using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace ExcelTranscript.Models
{
    public static class HelperEmail
    {
        public static bool SendEmail(string messages = "")
        {
            string html = messages;
            var fromAddress = new MailAddress("firstmedicines@gmail.com", "Excel");
            var toAddress = new MailAddress("info.h2svolt@gmail.com");
            const string fromPassword = "dparhakdzitxbyuy";
            const string subject = "Excel";
            string body = html;


            try
            {
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    IsBodyHtml = true,
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }

    public static class UserRoles
    {
        public static readonly string Provider = "Provider";
        public static readonly string Manager = "Manager";
        public static readonly string Typist = "Typist";
    }
}