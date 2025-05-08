using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Components;
using SharedLibrary.Events;
using SharedLibrary.Interfaces;
using SharedLibrary.JsInterop;
using SharedLibrary.Options;

namespace SharedLibrary.Services;

public static class AWComponentServices
{
    public static WebAssemblyHostBuilder AddClientServices(this WebAssemblyHostBuilder builder)
    {
        _ = builder.Services.AddLocalization(options =>
        {
            options.ResourcesPath = "Resources";
        });

        builder.Configuration.AddJsonFile("appsettings.json", optional: true);

        builder.Services.Configure<JsModuleOptions>(builder.Configuration.GetSection(JsModuleOptions.SectionName));

        builder.Services.AddSingleton<IEventBus, EventBus>();

        builder.Services.AddScoped<AWJsInterop>();

        builder.Services.AddSingleton<IUndoService, UndoService>();

        builder.Services.AddSingleton<IDiagramService, DiagramService>();

        return builder;
    }
}
