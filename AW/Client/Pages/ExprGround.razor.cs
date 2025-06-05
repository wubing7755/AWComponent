using AWUI.JsInterop;
using AWUI.Models;
using AWUI.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;

namespace AW.Client.Pages;

public partial class ExprGround
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public AWJsInterop AWJsInterop { get; set; }

    private IBrowserFile _selectedFile;
    private HubConnection _hubConnection { get; set; }
    private string _inputext = string.Empty;
    private string _translationText = string.Empty;
    private const string DividerStyle = "display: block; height: 30px; color:green;";

    private async Task ConvertToXML()
    {
        var xmlWriter = new XmlWriter(AWJsInterop);

        var person = new Person
        {
            Name = "hello",
            Age = "world"
        };

        await xmlWriter.WriteToFile(person, "Person.xml");
    }

    private async Task TranslateInputText(string text)
    {
        _inputext = text;
        _translationText = await Translator.TranslateAsync(text);

        StateHasChanged();
    }

    private void OnSelectedFileChanged(InputFileChangeEventArgs args)
    {
        _selectedFile = args.File;
    }

    private async Task StartUpLoad()
    {
        try
        {
            _hubConnection = new HubConnectionBuilder()
                    .WithUrl(NavigationManager.ToAbsoluteUri("/fileUploadHub"))
                    .WithAutomaticReconnect()
                    .Build();

            // 检查连接状态
            if (_hubConnection.State != HubConnectionState.Connected)
            {
                await _hubConnection.StartAsync();
            }

            // 定义分块大小 (1MB)
            const int chunkSize = 1 * 1024 * 1024;
            var totalChunks = (int)Math.Ceiling((double)_selectedFile.Size / chunkSize);
            // 创建缓冲区
            var buffer = new byte[chunkSize];

            using (var stream = _selectedFile.OpenReadStream(_selectedFile.Size))
            {
                for (int chunkIndex = 0; chunkIndex < totalChunks; chunkIndex++)
                {
                    var bytesRead = await stream.ReadAsync(buffer);
                    if (bytesRead == 0) break;
                    await _hubConnection.InvokeAsync("UploadChunk",
                        _selectedFile.Name,
                        chunkIndex,
                        (int)Math.Ceiling((double)_selectedFile.Size / chunkSize),
                        buffer.AsMemory(0, bytesRead).ToArray());
                }
            }
        }
        catch (Exception ex)
        {
            StateHasChanged();
        }
    }
}
