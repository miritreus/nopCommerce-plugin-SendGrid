using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Messages;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Plugin.Misc.SendGridEmailSender.Models;

namespace Nop.Plugin.Misc.SendGridEmailSender.Controllers;

[AutoValidateAntiforgeryToken]
public class SendGridEmailSenderController : BasePluginController
{
    #region Fields
   
    protected readonly IEmailAccountService _emailAccountService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILogger _logger;
    protected readonly IMessageTemplateService _messageTemplateService;
    protected readonly IMessageTokenProvider _messageTokenProvider;
    protected readonly INotificationService _notificationService;
    protected readonly ISettingService _settingService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IStoreService _storeService;
    protected readonly IWorkContext _workContext;
    protected readonly MessageTemplatesSettings _messageTemplatesSettings;
    protected readonly SendGridSettings _sendGridSettings;

    #endregion

    #region Ctor

    public SendGridEmailSenderController(IEmailAccountService emailAccountService,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        ILogger logger,
        IMessageTemplateService messageTemplateService,
        IMessageTokenProvider messageTokenProvider,
        INotificationService notificationService,
        ISettingService settingService,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        IStoreService storeService,
        IWorkContext workContext,
        MessageTemplatesSettings messageTemplatesSettings, SendGridSettings sendGridSettings)
    {     
        _emailAccountService = emailAccountService;
        _genericAttributeService = genericAttributeService;
        _localizationService = localizationService;
        _logger = logger;
        _messageTemplateService = messageTemplateService;
        _messageTokenProvider = messageTokenProvider;
        _notificationService = notificationService;
        _settingService = settingService;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
        _storeService = storeService;
        _workContext = workContext;
        _messageTemplatesSettings = messageTemplatesSettings;
        _sendGridSettings = sendGridSettings;
    }

    #endregion 

    #region Methods

    [AuthorizeAdmin]
    [Area(AreaNames.ADMIN)]
    public async Task<IActionResult> Configure()
    {
        var model = new ConfigurationModel();
     
        model.IsEnabled = _sendGridSettings?.IsEnabled ?? false;
        model.ClientSecret = _sendGridSettings?.ClientSecret;        

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
        sendGridSettings.ClientSecret = model.ClientSecret;

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

    
  
    #endregion
}