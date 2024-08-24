using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Services.Messages;


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

        if (settings?.IsEnabled ?? false)
        {
            services.AddScoped<IEmailSender, Services.SendGridEmailSender>();
        }
        else
        {
        }
    }
}
