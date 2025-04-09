using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Components;
using SharedLibrary.Events;
using SharedLibrary.JsInterop;

namespace SharedLibrary.Services;

public static class AWComponentServices
{
    public static WebAssemblyHostBuilder AddAWComponentServices(this WebAssemblyHostBuilder builder)
    {
        _ = builder.Services.AddLocalization(options =>
        {
            options.ResourcesPath = "Resources";
        });

        builder.Services.AddSingleton<IEventBus, EventBus>();

        builder.Services.AddScoped<AWJsInterop>();

        return builder;
    }
}
