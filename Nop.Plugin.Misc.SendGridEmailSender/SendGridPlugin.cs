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
            ["Plugins.Misc.SendGrid.IsEnabled"] = "Enabled",
            ["Plugins.Misc.SendGrid.ApiKey"] = "API key",
            ["Plugins.Misc.SendGrid.SettingsText"] = "Configure SendGrid email sender with your API key. When you change the value of enabled, the application will be restarted.",

        });

        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        await _settingService.DeleteSettingAsync<SendGridSettings>();

        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Misc.SendGrid");

        await base.UninstallAsync();
    }
}