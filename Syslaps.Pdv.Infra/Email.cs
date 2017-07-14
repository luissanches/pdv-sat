using System;
using System.Collections.Generic;
using System.Net.Mail;
using Syslaps.Pdv.Core;
using Syslaps.Pdv.Core.Dominio.Base;
using Syslaps.Pdv.Entity;
using Syslaps.Pdv.Infra.Repositorio;

namespace Syslaps.Pdv.Infra
{
    public class Email : IEmail
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _emailLogin;
        private readonly string _password;

        public Email(IRepositorioBase repositorioBase)
        {
            var parametros = repositorioBase.RecuperarTodos<Parametro>();
            _host = parametros.Find(x => x.Nome == "smtp").Valor;
            _port = Convert.ToInt32(parametros.Find(x => x.Nome == "smtp.port").Valor);
            _emailLogin = parametros.Find(x => x.Nome == "smtp.user").Valor;
            _password = parametros.Find(x => x.Nome == "smtp.password").Valor;
        }

        public void Enviar(string senderEmail, string senderName, string fromEmail, string fromName, string subject, string htmlBody, List<MailAddress> emailsTo)
        {
            var mail = new MailMessage();
            try
            {
                var client = new SmtpClient();
                client.Host = _host;
                client.Port = _port;
                client.EnableSsl = true;
                client.Credentials = new System.Net.NetworkCredential(_emailLogin, _password);

                mail.Sender = new MailAddress(senderEmail, senderName);
                mail.From = new MailAddress(fromEmail, fromName);
                foreach (var mailAddress in emailsTo)
                {
                    mail.To.Add(mailAddress);
                }

                mail.Subject = subject;
                mail.Body = htmlBody;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                client.Send(mail);

            }
            finally
            {
                mail.Dispose();
                mail = null;
            }
        }
    }
}
