using AW.Server.Hubs;

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
            // 10MBÎÄ¼þÇÐÆ¬£¬10MBÈßÓàÁ¿
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
