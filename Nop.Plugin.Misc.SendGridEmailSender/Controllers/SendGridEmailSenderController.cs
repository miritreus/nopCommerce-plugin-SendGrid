using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Plugin.Misc.SendGridEmailSender.Models;

namespace Nop.Plugin.Misc.SendGridEmailSender.Controllers;

[AutoValidateAntiforgeryToken]
public class SendGridEmailSenderController : BasePluginController
{
    protected readonly ILocalizationService _localizationService;
    protected readonly ILogger _logger;
    protected readonly INotificationService _notificationService;
    protected readonly ISettingService _settingService;
    protected readonly IStoreContext _storeContext;

    public SendGridEmailSenderController(
        ILocalizationService localizationService,
        ILogger logger,
        INotificationService notificationService,
        ISettingService settingService,
        IStoreContext storeContext,
        IStoreService storeService)
    {
        _localizationService = localizationService;
        _logger = logger;
        _notificationService = notificationService;
        _settingService = settingService;
        _storeContext = storeContext;
    }

    [AuthorizeAdmin]
    [Area(AreaNames.ADMIN)]
    public async Task<IActionResult> Configure()
    {
        var model = new ConfigurationModel();

        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var sendGridSettings = await _settingService.LoadSettingAsync<SendGridSettings>(storeId);

        model.IsEnabled = sendGridSettings?.IsEnabled ?? false;
        model.ApiKey = sendGridSettings?.ApiKey;

        return View("~/Plugins/Misc.SendGridEmailSender/Views/Configure.cshtml", model);
    }

    [AuthorizeAdmin]
    [Area(AreaNames.ADMIN)]
    [HttpPost, ActionName("Configure")]
    [FormValueRequired("save")]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
        {
            return await Configure();
        }

        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();

        var sendGridSettings = await _settingService.LoadSettingAsync<SendGridSettings>(storeId);

        var enabledPreviousValue = sendGridSettings.IsEnabled;

        sendGridSettings.IsEnabled = model.IsEnabled;
        sendGridSettings.ApiKey = model.ApiKey;

        await _settingService.SaveSettingAsync(sendGridSettings);
        await _settingService.ClearCacheAsync();

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        // Restart the application if there is a chnage in the value of enabled
        if (enabledPreviousValue != model.IsEnabled)
        {
            return View("RestartApplication", Url.Action("List", "Plugin"));
        }
        else
        {
            return await Configure();
        }
    }
}