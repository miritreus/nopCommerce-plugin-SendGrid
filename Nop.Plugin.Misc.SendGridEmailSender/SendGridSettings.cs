using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.SendGridEmailSender;
public class SendGridSettings : ISettings
{
    public bool IsEnabled { get; set; }

    public string ApiKey { get; set; }
}
