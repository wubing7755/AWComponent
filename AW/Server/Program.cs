using AW.Server.Hubs;
using AW.Server.Middlewares;

namespace AW;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();

        // add signalR service
        builder.Services.AddSignalR(hubOptions =>
        {
            // 10MB文件切片，10MB冗余量
            hubOptions.MaximumReceiveMessageSize = 20 * 1024 * 1024;
            hubOptions.ClientTimeoutInterval = TimeSpan.FromMinutes(5);
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        // add 自定义中间件
        app.Use(async (context, next) =>
        {
            // Do work that can write to the Response.
            await next.Invoke();
            // Do logging or other work that doesn't write to the Response.
        });

        app.UseMiddleware<LoggingMiddleware>();

        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseRouting();

        app.MapHub<FileUploadHub>("/fileUploadHub");

        app.MapRazorPages();
        app.MapControllers();
        app.MapFallbackToFile("index.html");

        app.Run();
    }
}
