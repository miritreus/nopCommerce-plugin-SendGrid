using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.SendGridEmailSender;
public class SendGridSettings : ISettings
{
    public bool IsEnabled { get; set; }

    public string ClientSecret { get; set; }
}
