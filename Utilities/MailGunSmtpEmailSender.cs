using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Security.Authentication;
using MailKit.Net.Smtp;
using MimeKit;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace conversion_api.Utilities
{
    public class MailGunSmtpEmailSender : IEmailSender
    {
        //
        // Identify the connection type to the MailGun server
        //
        public const string ConnectionType = "smtp";

        // 
        // Identify the external mail service provider
        // 
        public const string Provider = "MailGun";

        // 
        // Configuration setting for routing email through MailGun SMTP.
        //
        private readonly MailGunSmtpEmailSettings _smtpEmailConfig;
        private readonly IWebHostEnvironment _webHostEnvironment;

        //
        // Accessor for the configuration to send mail via MailGun SMTP
        // 
        public MailGunSmtpEmailSettings EmailSettings
        {
            get
            {
                return _smtpEmailConfig;
            }
        }

        // 
        // Default constructor with email configuration initialized via
        // IOption.
        // 
        public MailGunSmtpEmailSender(
            IOptions<MailGunSmtpEmailSettings> smtpEmailConfig,
            IWebHostEnvironment webHostEnvironment
        )
        {
            _smtpEmailConfig = smtpEmailConfig.Value;
            _webHostEnvironment = webHostEnvironment;
        }

        // 
        // Sends email via MailGun SMTP, given email recipient, email
        // subject, and email body.
        // 
        public async Task SendEmailAsync(
            string email,
            string subject,
            string message
        )
        {
            string host = EmailSettings.SmtpHost;
            int port = EmailSettings.SmtpPort;
            string login = EmailSettings.SmtpLogin;
            string code = EmailSettings.SmtpPassword;
            string senderName = EmailSettings.SenderName;
            string sender = EmailSettings.From;

            var emailMsg = new MimeMessage();
            emailMsg.From.Add(new MailboxAddress(senderName, sender));
            emailMsg.To.Add(new MailboxAddress("", email));
            emailMsg.Subject = subject;

            string htmlTemplate = readHTMLFile("EmailTemplates/Contact-Email.html");
            htmlTemplate = htmlTemplate.Replace("{body}", message);

            emailMsg.Body = new TextPart("html")
            {
                Text = htmlTemplate
            };

            using (SmtpClient client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(host, port, false)
                        .ConfigureAwait(false);

                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(login, code);
                    await client.SendAsync(emailMsg).ConfigureAwait(false);
                }
                catch (AuthenticationException ex)
                {
                    Console.WriteLine("Error: Authentication exception.");
                    Console.WriteLine("\tException message: {0}", ex.Message);

                }
                catch (SmtpCommandException ex)
                {
                    Console.WriteLine("Error: SMTP exception.");
                    Console.WriteLine("\tException message: {0}", ex.Message);
                    Console.WriteLine("\tStatus code: {0}", ex.StatusCode);

                    switch (ex.ErrorCode)
                    {

                        case SmtpErrorCode.RecipientNotAccepted:
                            Console.WriteLine(
                                "Error: Recipient not accepted: {0}",
                                ex.Mailbox
                            );
                            break;

                        case SmtpErrorCode.SenderNotAccepted:
                            Console.WriteLine(
                                "Error: Sender not accepted: {0}",
                                ex.Mailbox
                            );
                            break;

                        case SmtpErrorCode.MessageNotAccepted:
                            Console.WriteLine("Error: Message not accepted.");
                            break;

                        default:
                            Console.WriteLine("Error: {0}.", ex.Message);
                            break;
                    }
                }
                catch (SmtpProtocolException ex)
                {
                    Console.WriteLine("Error: SMTP protocol exception.");
                    Console.WriteLine("\tException message: {0}", ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: Failed to send mail via {0} {1}.",
                        Provider,
                        ConnectionType
                    );
                }
                finally
                {
                    if (true == client.IsConnected)
                    {
                        await client.DisconnectAsync(true)
                            .ConfigureAwait(false);
                    }
                }

                if (true == client.IsConnected)
                {
                    await client.DisconnectAsync(true).ConfigureAwait(false);
                }
            }
        }

        private string readHTMLFile(string htmlFileName)
        {
            string path = _webHostEnvironment.WebRootPath + htmlFileName;
            return File.ReadAllText(path);
        }
    }

    public class MailGunSmtpEmailSettings
    {
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpLogin { get; set; }
        public string SmtpPassword { get; set; }
        public string SenderName { get; set; }
        public string From { get; set; }
    }

    public class EmailSettings
    {
        public string ApiKey { get; set; }
        public string BaseUri { get; set; }
        public string RequestUri { get; set; }
        public string From { get; set; }
        public string Hostname { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
