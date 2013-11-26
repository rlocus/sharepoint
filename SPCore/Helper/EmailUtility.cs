using System.Collections.Generic;
using Microsoft.SharePoint.Administration;
using System.Net;
using System.Net.Mail;

namespace SPCore.Helper
{
    public static class EmailUtility
    {
        public static void SendEmail(string from, string to, string subject, string body, bool isBodyHtml, IEnumerable<Attachment> attachments, bool enableSsl = false, int smtpPort = 25)
        {
            SendEmail(from, to, subject, body, isBodyHtml, CredentialCache.DefaultNetworkCredentials, attachments, enableSsl, smtpPort);
        }

        public static void SendEmail(string to, string subject, string body, bool isBodyHtml, IEnumerable<Attachment> attachments, bool enableSsl = false, int smtpPort = 25)
        {
            SendEmail(to, subject, body, isBodyHtml, CredentialCache.DefaultNetworkCredentials, attachments, enableSsl, smtpPort);
        }

        public static void SendEmail(string to, string subject, string body, bool isBodyHtml, NetworkCredential credential, IEnumerable<Attachment> attachments, bool enableSsl = false, int smtpPort = 25)
        {
            SendEmail(SPAdministrationWebApplication.Local.OutboundMailSenderAddress, to, subject, body, isBodyHtml, credential, attachments, enableSsl, smtpPort);
        }

        public static void SendEmail(string from, string to, string subject, string body, bool isBodyHtml, NetworkCredential credential,
                                      IEnumerable<Attachment> attachments, bool enableSsl = false, int smtpPort = 25)
        {
            using (var message = new MailMessage(from, to)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = isBodyHtml
            })
            {
                if (attachments != null)
                {
                    foreach (Attachment attachment in attachments)
                    {
                        message.Attachments.Add(attachment);
                    }
                }

                SendEmail(message, credential, enableSsl, smtpPort);
            }
        }

        public static void SendEmail(MailMessage message, NetworkCredential credential, bool enableSsl = false, int smtpPort = 25)
        {
            string smtpServer = SPAdministrationWebApplication.Local.OutboundMailServiceInstance.Server.Address;
            SmtpClient client = new SmtpClient(smtpServer, smtpPort);
            //using (client)
            //{
            client.EnableSsl = enableSsl;
            //client.UseDefaultCredentials = true;
            //client.DeliveryMethod = SmtpDeliveryMethod.Network;

            if (client.EnableSsl)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;
            }

            client.Credentials = credential;
            client.Send(message);
            //}
        }
    }
}
