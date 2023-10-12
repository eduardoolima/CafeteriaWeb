using Azure;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace CafeteriaWeb.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger _logger;

        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor,
                           ILogger<EmailSender> logger)
        {
            Options = optionsAccessor.Value;
            _logger = logger;
        }

        public AuthMessageSenderOptions Options { get; } //Set with Secret Manager.



        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            await Execute(subject, message, toEmail);        
        }

        public async Task Execute(string subject, string message, string toEmail)
        {
            var smtpServer = Options.SmtpServer;
            var smtpPort = 587; // Porta SMTP típica para envio seguro (TLS)
            var smtpUsername = Options.SmtpUserName;
            var smtpPassword = Options.SmtpPassword;

            // Endereço de email do remetente
            var fromAddress = new MailAddress(Options.EmailFrom, Options.EmailDisplayName);

            // Endereço de email do destinatário
            var toAddress = new MailAddress(toEmail);

            // Configuração do cliente SMTP
            using (var smtpClient = new SmtpClient
            {
                Host = smtpServer,
                Port = smtpPort,
                EnableSsl = true, //SSL/TLS para conexão segura
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
            })
            {
                // Crie a mensagem de email
                var msg = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    IsBodyHtml = true,
                    Body = message,
                };
                string result = string.Empty;
                try
                {
                    // Envie o email
                    smtpClient.Send(msg);
                    result = "Email sent successfully!";
                }
                catch (Exception ex)
                {
                    result = $"Failure Email to {toEmail}: {ex.Message}";
                    throw;
                }
                finally
                {
                    _logger.LogInformation(result);
                }
            }
                        
        }
    }
}
