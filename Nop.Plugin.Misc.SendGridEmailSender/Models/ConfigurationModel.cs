using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.SendGridEmailSender.Models;

public record ConfigurationModel : BaseNopModel
{
    [NopResourceDisplayName(Constants.ISENABLED_FIELD_RESOURCENAME)]
    public bool IsEnabled { get; set; }

    [NopResourceDisplayName(Constants.APIKEY_FIELD_RESOURCENAME)]
    public string ApiKey { get; set; }
}