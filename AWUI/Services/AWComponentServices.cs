using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AWUI.Events;
using AWUI.Interfaces;
using AWUI.JsInterop;
using AWUI.Options;

namespace AWUI.Services;

public static class AWComponentServices
{
    public static WebAssemblyHostBuilder AddClientServices(this WebAssemblyHostBuilder builder)
    {
        _ = builder.Services.AddLocalization(options =>
        {
            options.ResourcesPath = "Resources";
        });
        
        builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        // 从配置文件中读取 JsModules 配置节并将其作为 JsModuleOptions 类型注册到依赖注入系统中，
        // 以便在其他服务中通过 IOptions<JsModuleOptions> 注入使用。
        builder.Services.Configure<JsModuleOptions>(ops =>
        {
            var section = builder.Configuration.GetSection(JsModuleOptions.SectionName);
            ops.Version = section["Version"] ?? "0.0.2";

            var modules = section.GetSection("Modules").GetChildren();
            foreach (var module in modules) 
            {
                ops.Modules.Add(new JsModuleOption
                {
                    Name = module["Name"] ?? throw new InvalidOperationException("模块名不能为空"),
                    Path = module["Path"] ?? throw new InvalidOperationException("模块路径不能为空"),
                    Enable = bool.TryParse(module["Enable"], out var enable) && enable
                });
            }
        
        });

        builder.Services.AddSingleton<IEventBus, EventBus>();

        builder.Services.AddScoped<AWJsInterop>();

        builder.Services.AddSingleton<IUndoService, UndoService>();

        return builder;
    }
}
