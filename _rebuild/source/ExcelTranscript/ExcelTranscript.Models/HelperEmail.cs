using System;
using System.Net;
using System.Net.Mail;

namespace ExcelTranscript.Models
{
	public static class HelperEmail
	{
		public static bool SendEmail(string messages = "")
		{
			MailAddress mailAddress = new MailAddress("firstmedicines@gmail.com", "Excel");
			MailAddress to = new MailAddress("farooq.mangotech@gmail.com");
			try
			{
				SmtpClient smtpClient = new SmtpClient
				{
					Host = "smtp.gmail.com",
					Port = 587,
					EnableSsl = true,
					DeliveryMethod = SmtpDeliveryMethod.Network,
					UseDefaultCredentials = false,
					Credentials = new NetworkCredential(mailAddress.Address, "dparhakdzitxbyuy")
				};
				using (MailMessage message = new MailMessage(mailAddress, to)
				{
					IsBodyHtml = true,
					Subject = "Excel",
					Body = messages
				})
				{
					smtpClient.Send(message);
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
