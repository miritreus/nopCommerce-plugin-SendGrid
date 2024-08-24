using Nop.Core.Domain.Messages;
using Nop.Services.Messages;
using SendGrid.Helpers.Mail;
using SendGrid;
using Nop.Services.Configuration;
using Nop.Core;

namespace Nop.Plugin.Misc.SendGridEmailSender.Services;
public class SendGridEmailSender : IEmailSender
{
    protected readonly ISettingService _settingService;
    protected readonly IStoreContext _storeContext;

    public SendGridEmailSender(ISettingService settingsService, IStoreContext storeContext)
    { 
        _settingService = settingsService;
        _storeContext = storeContext;
    }

    public async Task SendEmailAsync(EmailAccount emailAccount, string subject, string body, string fromAddress, string fromName
        , string toAddress, string toName, string replyToAddress = null, string replyToName = null, IEnumerable<string> bcc = null
        , IEnumerable<string> cc = null, string attachmentFilePath = null, string attachmentFileName = null, int attachedDownloadId = 0
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

        var plainTextContent = "and easy to do anywhere, even with C#";
        var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";

        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

        var response = await client.SendEmailAsync(msg);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error while sending email with SendGrid. Statuscode is {response.StatusCode}.");
        }
    }
}
