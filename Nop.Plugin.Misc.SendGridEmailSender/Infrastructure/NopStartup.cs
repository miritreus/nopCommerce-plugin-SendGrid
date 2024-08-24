using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nop.Core.Infrastructure;
using Nop.Data.Migrations;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Org.BouncyCastle.Pqc.Crypto.Lms;

namespace Nop.Plugin.Misc.SendGridEmailSender.Infrastructure;
public class NopStartup : INopStartup
{
    public int Order => 5000;

    public void Configure(IApplicationBuilder application)
    {
        
    }

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        using var scope = services.BuildServiceProvider().CreateScope();

        var settings = scope.ServiceProvider.GetService<SendGridSettings>();
        //var logger = scope.ServiceProvider.GetService<Nop.Services.Logging.ILogger>();

        if (settings?.IsEnabled ?? false)
        {
            services.AddScoped<IEmailSender, Services.SendGridEmailSender>();

            //logger.Information("SendGrid email sender is enabled and will  be used!");
        }
        else
        {
            //logger.Information("SendGrid email sender is not enabled and will not be used!");
        }
    }
}
