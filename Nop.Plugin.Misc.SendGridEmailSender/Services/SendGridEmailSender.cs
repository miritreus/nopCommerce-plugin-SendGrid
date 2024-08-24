using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Nop.Core.Domain.Messages;
using Nop.Services.Messages;
using SendGrid.Helpers.Mail;
using SendGrid;

namespace Nop.Plugin.Misc.SendGridEmailSender.Services;
public class SendGridEmailSender : IEmailSender
{
    public async Task SendEmailAsync(EmailAccount emailAccount, string subject, string body, string fromAddress, string fromName
        , string toAddress, string toName, string replyToAddress = null, string replyToName = null, IEnumerable<string> bcc = null
        , IEnumerable<string> cc = null, string attachmentFilePath = null, string attachmentFileName = null, int attachedDownloadId = 0
        , IDictionary<string, string> headers = null)
    {
        var apiKey = "KEY";
        var client = new SendGridClient(apiKey);
        var from = new EmailAddress("test@example.com", "Example User");      

        var to = new EmailAddress("test@example.com", "Example User");
        var plainTextContent = "and easy to do anywhere, even with C#";
        var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        var response = await client.SendEmailAsync(msg);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error while sending email with SendGrid. Statuscode is {response.StatusCode}");
        }
    }
}
