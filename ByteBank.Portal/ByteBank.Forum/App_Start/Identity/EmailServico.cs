using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Mail;
using System.Net;

namespace ByteBank.Forum.App_Start.Identity
{
    public class EmailServico : IIdentityMessageService
    {
        private readonly string EMAIL_ORIGEM = ConfigurationManager.AppSettings["emailServico:email_Remetente"];
        private readonly string EMAIL_SENHA = ConfigurationManager.AppSettings["emailServico:email_senha"];

        public async Task SendAsync(IdentityMessage message)
        {
            using (var mensagemDeEmail = new MailMessage())
            {
                mensagemDeEmail.From = new MailAddress(EMAIL_ORIGEM);    
                mensagemDeEmail.Subject = message.Subject;
                mensagemDeEmail.To.Add(message.Destination);
                mensagemDeEmail.Body = message.Body;
                using (var SmtpClient = new SmtpClient()) {

                    SmtpClient.UseDefaultCredentials = true;
                    SmtpClient.Credentials = new NetworkCredential(EMAIL_ORIGEM, EMAIL_SENHA);

                    SmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    SmtpClient.Host = "smtp.google.com";
                    SmtpClient.Port = 587;
                    SmtpClient.EnableSsl = true;
                    SmtpClient.Timeout = 20000;

                    await SmtpClient.SendMailAsync(mensagemDeEmail);
                }
            }
        }
    }
}