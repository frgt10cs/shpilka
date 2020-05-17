using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace BotRules.Services
{
    public static class EmailManager
    {                
        public static string EmailSender;
        public static string PasswordSender;        

        /// <summary>
        /// Отправка сообщения на почту
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="messageBody"></param>
        /// <param name="messageSubject"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static async Task SendMessageAsync(string recipient, string messageBody = "Empty message", string messageSubject = "Empty subject",  string host = "smtp.gmail.com", int port = 587)
        {            
            var client = new SendGridClient("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");
            var from = new EmailAddress(EmailSender);
            var to = new EmailAddress(recipient);
            var body = messageBody;
            var msg = MailHelper.CreateSingleEmail(from, to, "Подтверждение регистрации на проекте Shpilka", body, "");
            await client.SendEmailAsync(msg);
        }     
    }
}