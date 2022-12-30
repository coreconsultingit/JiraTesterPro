using System.Diagnostics;
using System.Net.Mail;
using Attachment = System.Net.Mail.Attachment;
using PreMailer.Net;
namespace JiraTesterProService.MailHandler;

public class EmailSender : IEmailSender
{
    private IConfiguration configuration;
    private ILogger<EmailSender> logger;
    public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
    {
        this.configuration = configuration;
        this.logger = logger;
    }

    public bool? IsBodyHtml { get; set; }
    public void SendMessage(string to, string subject, string body, params string[] attachments)
    {
        try
        {
            var smtpServer = configuration.GetValue<string>("Email:SmtpServer");
            var from = configuration.GetValue<string>("Email:Sender");
            var sendMail = configuration.GetValue<bool>("Email:SendMail");

            var toAddresses = to.Replace(';', ',');

            using (var smtp = new SmtpClient(smtpServer))
            {
                var message = new MailMessage()
                {
                    Subject = subject,
                    IsBodyHtml = IsBodyHtml??true,
                    Body = PreMailer.Net.PreMailer.MoveCssInline(body).Html


            };
                message.From = new MailAddress(from);
                foreach (string address in toAddresses.Split(','))
                {
                    MailAddress toAddress = new MailAddress(address);
                    message.To.Add(toAddress);
                }

                if (attachments != null && attachments.Any())
                {
                    foreach (var attachmentPath in attachments)
                    {
                        var attachment = new Attachment(attachmentPath);
                        message.Attachments.Add(attachment);
                    }
                }

                

                logger.LogInformation("Sending Email...");
                logger.LogInformation(message.To.ToString());
                logger.LogInformation(message.Subject);
                logger.LogInformation(message.Body);
                if (!Debugger.IsAttached && sendMail)
                {
                    smtp.Send(message);
                }
            }
        }
        catch (Exception ex)
        {
            // Only log E-Mail exceptions.
            logger.LogError($"Unable to send email: {ex.Message} {ex.StackTrace} {ex.InnerException?.Message} {ex.InnerException?.StackTrace}");
        }
    }
}