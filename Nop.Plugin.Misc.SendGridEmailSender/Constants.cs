
namespace Nop.Plugin.Misc.SendGridEmailSender;
public static class Constants
{
    public const string RESOURCES_PREFIX = "Plugins.Misc.SendGrid";

    public const string ISENABLED_FIELD_RESOURCENAME = $"{RESOURCES_PREFIX}.Fields.IsEnabled";
    public const string ISENABLED_FIELD_HINT_RESOURCENAME = $"{ISENABLED_FIELD_RESOURCENAME}.Hint";

    public const string APIKEY_FIELD_RESOURCENAME = $"{RESOURCES_PREFIX}.Fields.ApiKey";
    public const string APIKEY_FIELD_HINT_RESOURCENAME = $"{APIKEY_FIELD_RESOURCENAME}.Hint";

    public const string APIKEY_FIELD_VALUE_REQUIRED_RESOURCENAME = $"{RESOURCES_PREFIX}.Fields.ApiKey.IsRequired";
}
