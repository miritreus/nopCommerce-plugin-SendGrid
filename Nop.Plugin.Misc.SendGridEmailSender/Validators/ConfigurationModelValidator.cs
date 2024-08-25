using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Plugin.Misc.SendGridEmailSender.Models;

namespace Nop.Plugin.Misc.SendGridEmailSender.Validators;

/// <summary>
/// Represents configuration model validator
/// </summary>
public class ConfigurationModelValidator : BaseNopValidator<ConfigurationModel>
{
    #region Ctor

    public ConfigurationModelValidator(ILocalizationService localizationService)
    {
        RuleFor(model => model.ApiKey)
            .NotEmpty().When(model => model.IsEnabled)
            .WithMessageAwait(localizationService.GetResourceAsync(Constants.APIKEY_FIELD_VALUE_REQUIRED_RESOURCENAME));
    }

    #endregion
}