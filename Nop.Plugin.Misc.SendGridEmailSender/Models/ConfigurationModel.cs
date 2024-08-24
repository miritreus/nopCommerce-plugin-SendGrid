using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.SendGridEmailSender.Models;

/// <summary>
/// Represents plugin configuration model
/// </summary>
public record ConfigurationModel : BaseNopModel
{
    [NopResourceDisplayName("Plugins.Misc.SendGrid.Enabled")]
    public bool IsEnabled { get; set; }

    [NopResourceDisplayName("Plugins.Misc.SendGrid.ClientSecret")]
    public string ClientSecret { get; set; }
}