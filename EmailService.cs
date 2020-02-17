using MailKit.Net.Smtp;
using MimeKit;
using System.Collections.Generic;

namespace SchoolSMS
{
    public class EmailService
    {
        private string host;
        private int port;
        private bool isSsl;
        private string username;
        private string password;

        static SmtpClient smtp = new SmtpClient();
        static MimeMessage message = new MimeMessage();

        public EmailService(string host, int port, bool isSsl)
        {
            smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
            smtp.Connect(this.host = host, this.port = port, this.isSsl = isSsl);
            // Note: since we don't have an OAuth2 token, disable  
            // the XOAUTH2 authentication mechanism.  
            smtp.AuthenticationMechanisms.Remove("XOAUTH2");
        }
        public void Autenticate(string username, string password)
        {
            // Note: only needed if the SMTP server requires authentication  
            smtp.Authenticate(this.username = username, this.password = password);
            message.From.Add(new MailboxAddress(username, username));
        }

        public bool Send(IList<string> recipients, string subject, string body, bool ishtml)
        {
            message.To.Clear();
            foreach (string recipient in recipients)
            {
                message.To.Add(new MailboxAddress(recipient, recipient));
            }

            message.Subject = subject;
            string bodyFormat = ishtml ? "html" : "plain";
            message.Body = new TextPart(bodyFormat)
            {
                Text = body
            };

            if (!smtp.IsConnected)
            {
                smtp.Connect(host, port, isSsl);
                Autenticate(username, password);
            }

            smtp.Send(message);

            return true;
        }
        public void Close()
        {
            smtp.Disconnect(true);
        }
    }
}