using System.Collections.Generic;
using System.Net.Mail;

namespace Syslaps.Pdv.Core.Dominio.Base
{
    public interface IEmail
    {
        void Enviar(string senderEmail, string senderName, string fromEmail, string fromName, string subject, string htmlBody, List<MailAddress> emailsTo);
    }
}