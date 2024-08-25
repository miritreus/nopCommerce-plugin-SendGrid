using Nop.Core;
using Nop.Plugin.Misc.SendGridEmailSender;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.ExternalAuth.Facebook;

/// <summary>
/// Represents method for the authentication with Facebook account
/// </summary>
public class SendGridPlugin : BasePlugin, IMiscPlugin
{
    protected readonly ILocalizationService _localizationService;
    protected readonly ISettingService _settingService;
    protected readonly IWebHelper _webHelper;

    public SendGridPlugin(ILocalizationService localizationService, ISettingService settingService, IWebHelper webHelper)
    {
        _localizationService = localizationService;
        _settingService = settingService;
        _webHelper = webHelper;
    }

    public override string GetConfigurationPageUrl()
    {
        return $"{_webHelper.GetStoreLocation()}Admin/SendGridEmailSender/Configure";
    }

    public override async Task InstallAsync()
    {
        //settings
        await _settingService.SaveSettingAsync(new SendGridSettings());

        //locales
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            [Constants.ISENABLED_FIELD_RESOURCENAME] = "Enabled",
            [Constants.ISENABLED_FIELD_HINT_RESOURCENAME] = "Enable/disable the plugin. The application will be restarted.",
            [Constants.APIKEY_FIELD_RESOURCENAME] = "API key",
            [Constants.APIKEY_FIELD_HINT_RESOURCENAME] = "Enter your API key here. You can find it in SendGrid.",
            [Constants.APIKEY_FIELD_VALUE_REQUIRED_RESOURCENAME] = "API key is required"          
        });

        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        await _settingService.DeleteSettingAsync<SendGridSettings>();

        await _localizationService.DeleteLocaleResourcesAsync(Constants.RESOURCES_PREFIX);

        await base.UninstallAsync();
    }
}