using Nop.Core.Domain.Messages;
using Nop.Services.Messages;
using SendGrid.Helpers.Mail;
using SendGrid;
using Nop.Services.Configuration;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Media;
using Nop.Core.Domain.Media;

namespace Nop.Plugin.Misc.SendGridEmailSender.Services;
public class SendGridEmailSender : IEmailSender
{
    protected readonly ISettingService _settingService;
    protected readonly IStoreContext _storeContext;
    protected readonly INopFileProvider _fileProvider;
    protected readonly IDownloadService _downloadService;


    public SendGridEmailSender(ISettingService settingsService, IStoreContext storeContext, INopFileProvider fileProvider, IDownloadService downloadService)
    { 
        _settingService = settingsService;
        _storeContext = storeContext;
        _fileProvider = fileProvider;
        _downloadService = downloadService;
    }


    public async Task SendEmailAsync(EmailAccount emailAccount
                        , string subject, string body
                        , string fromAddress, string fromName, string toAddress, string toName, string replyToAddress = null, string replyToName = null
                        , IEnumerable<string> bcc = null, IEnumerable<string> cc = null
                        , string attachmentFilePath = null, string attachmentFileName = null, int attachedDownloadId = 0
                        , IDictionary<string, string> headers = null)
    {
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var sendGridSettings = await _settingService.LoadSettingAsync<SendGridSettings>(storeId);

        if (sendGridSettings == null || string.IsNullOrEmpty(sendGridSettings.ApiKey))
        {
            throw new Exception("SendGrid plugin is not correctly configured. Make sure an APIkey is configured.");
        }
           
        var client = new SendGridClient(sendGridSettings.ApiKey);

        var from = new EmailAddress(fromAddress, fromName); 
        var to = new EmailAddress(toAddress, toName);
       
        var message = MailHelper.CreateSingleEmail(from, to, subject, body, body);

        // Add reply to
        if (!string.IsNullOrEmpty(replyToAddress))
        { 
            message.SetReplyTo(new EmailAddress(replyToAddress, replyToName));
        }
        
        // Add BCC
        foreach (var address in (bcc ?? Enumerable.Empty<string>()).Where(m => !string.IsNullOrWhiteSpace(m)))
        { 
            message.AddBcc(new EmailAddress(address.Trim()));
        }

        // Add CC        
        foreach (var address in (cc ?? Enumerable.Empty<string>()).Where(m => !string.IsNullOrWhiteSpace(m)))
        {
            message.AddCc(new EmailAddress(address.Trim()));
        }

        // Headers
        if (headers != null)
        {        
            foreach (var header in headers)
            {
                message.Headers.Add(header.Key, header.Value);
            }
        }

        // Attachments
        if (!string.IsNullOrEmpty(attachmentFilePath) && _fileProvider.FileExists(attachmentFilePath))
        {
            if (string.IsNullOrWhiteSpace(attachmentFileName))
            {
                attachmentFileName = Path.GetFileName(attachmentFilePath);
            }

            var contents = await _fileProvider.ReadAllBytesAsync(attachmentFilePath);

            var attachment = CreateAttachment(attachmentFileName, contents);

            if (attachment != null)
            { 
                message.Attachments.Add(attachment);
            }
        }

        // A download attachment?
        if (attachedDownloadId > 0)
        {
            var download = await _downloadService.GetDownloadByIdAsync(attachedDownloadId);

            // We do not support URLs as attachments
            if (download != null && !download.UseDownloadUrl)
            {
                var fileName = !string.IsNullOrWhiteSpace(download.Filename) ? download.Filename : download.Id.ToString();
                fileName = $"{fileName}{download.Extension}";

                var attachment = CreateAttachment(fileName, download.DownloadBinary);

                if (attachment != null)
                {
                    message.Attachments.Add(attachment);
                }
            }
        }

        var response = await client.SendEmailAsync(message);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error while sending email with SendGrid. Statuscode is {response.StatusCode}.");
        }
    }

    private Attachment CreateAttachment(string attachmentFileName, byte[] contents)
    {   
        return new Attachment()
        {
            Filename = attachmentFileName,
            Disposition = "attachment",
            Type = MimeKit.MimeTypes.GetMimeType(attachmentFileName),
            Content = Convert.ToBase64String(contents)
        };
    }
}

