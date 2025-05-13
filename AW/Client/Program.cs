using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AWUI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace AW.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            // ��� SignalR �ͻ��˷���
            builder.Services.AddScoped(provider =>
            {
                var navigationManager = provider.GetRequiredService<NavigationManager>();
                return new HubConnectionBuilder()
                    .WithUrl(navigationManager.ToAbsoluteUri("/fileUploadHub"))
                    .WithAutomaticReconnect() // �Զ�����
                    .Build();
            });

            builder.AddClientServices();

            await builder.Build().RunAsync();
        }
    }
}
