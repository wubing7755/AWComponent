using AWUI.Components;
using AWUI.Models;
using AWUI.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;

namespace AW.Client.Pages;

public partial class AWComponents
{
    [Inject]
    public HubConnection HubConnection { get; set; } = default!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

    private bool _radioValue = false;

    private bool _alertVisible = false;

    private ElementReference _fileInputRef;

    private ElementReference _filesInputRef;

    private ModalDialog _dialog = new ModalDialog();

    private HashSet<string> Options = new HashSet<string>()
    {
        "option 1",
        "option 2",
        "option 3",
        "option 4",
        "option 5"
    };
    public string SelectedValue { get; set; } = string.Empty;

    private Input<string> _input = new Input<string>();

    private string _inputText { get; set; } = string.Empty;

    private UpFileInfo _upFileInfo;

    private TreeNode _treeNode = new TreeNode("展开项标题")
    {
        Children = new List<TreeNode>
            {
                new TreeNode("子项1")
                {
                    Children = new List<TreeNode>
                    {
                        new TreeNode("子项1-1"),
                        new TreeNode("子项1-2")
                    }
                },
                new TreeNode("子项2")
                {
                    Children = new List<TreeNode>
                    {
                        new TreeNode("子项2-1"),
                        new TreeNode("子项2-2")
                    }
                }
            }
    };

    private TreeNode _selectedTreeNode;

    private void TreeNodeSelected(TreeNode node)
    {
        _selectedTreeNode = node;
        StateHasChanged();
    }

    private async Task TestConnection(MouseEventArgs args)
    {
        await awJsInterop.TestConnection();
    }

    private async Task CheckBoxValueChanged(bool arg)
    {
        _radioValue = !_radioValue;
        StateHasChanged();
    }

    private Task OnTextInput(string value)
    {
        Console.WriteLine($"Key Enter: {value}");
        return Task.CompletedTask;
    }

    private Task AlertWarningBox(MouseEventArgs args)
    {
        _alertVisible = true;
        StateHasChanged();
        return Task.CompletedTask;
    }

    private Task CloseWarningBox()
    {
        Task.Delay(3000);
        _alertVisible = false;
        StateHasChanged();
        return Task.CompletedTask;
    }

    private async Task OnFileChanged(UpFileInfo upFileInfo)
    {
        _upFileInfo = upFileInfo;
        var textHelper = new TextFileHelper();
        await textHelper.DecodingText(upFileInfo);
    }

    private void OnFileInputRefChanged(ElementReference elementRef)
    {
        _fileInputRef = elementRef;
    }

    private void OnFilesInputRefChanged(ElementReference elementRef)
    {
        _filesInputRef = elementRef;
    }

    private async Task UpLoadFile(MouseEventArgs args)
    {
        var url = $"{Navigation.BaseUri}api/upload/chunk";
        await awJsInterop.UploadFileAsync(_fileInputRef, url);
    }

    protected override async Task OnInitializedAsync()
    {
        HubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/fileUploadHub"))
            .Build();
       
        await HubConnection.StartAsync();
    }

    private async Task UpLoadFileSingalR(MouseEventArgs args)
    {
        var stream = _upFileInfo.OpenReadStream();
        var size = _upFileInfo.Size;
        var fileName = _upFileInfo.Name;
        const int chunkSize = 1024 * 1024 * 2;
        var totalChunks = (int)Math.Ceiling((double)size / chunkSize);
        var buffer = new byte[chunkSize];
        var cancellationTokenSource = new CancellationTokenSource();
        
        try
        {
            // 通知服务端开始上传
            await HubConnection.SendAsync("StartUpload", fileName, size, totalChunks);
            for (int chunkIndex = 0; chunkIndex < totalChunks; chunkIndex++)
            {
                if (cancellationTokenSource.IsCancellationRequested) break;
                var bytesRead = await stream.ReadAsync(buffer, 0, chunkSize, cancellationTokenSource.Token);
                var actualChunk = new byte[bytesRead];
                Array.Copy(buffer, actualChunk, bytesRead);
                // 上传分块
                await HubConnection.SendAsync("UploadChunk",
                    fileName,
                    chunkIndex,
                    totalChunks,
                    actualChunk);
                // 更新进度 (可选)
                var progress = (int)((chunkIndex + 1) * 100 / totalChunks);
                Console.WriteLine($"上传进度: {progress}%");
            }
            // 通知服务端完成上传
            await HubConnection.SendAsync("CompleteUpload", fileName);
        }
        catch (Exception ex)
        {
            // 错误处理
            await HubConnection.SendAsync("AbortUpload", fileName);
            Console.WriteLine($"上传失败: {ex.Message}");
        }
        finally
        {
            cancellationTokenSource.Dispose();
            await stream.DisposeAsync();
        }

    }

    private async Task UpLoadFiles(MouseEventArgs args)
    {
        var url = $"{Navigation.BaseUri}api/upload/chunk";
        await awJsInterop.UploadFilesAsync(_filesInputRef, url);
    }

    private Task ShowDialog(MouseEventArgs args)
    {
        _dialog.SetVisible(true);
        return Task.CompletedTask;
    }

    private Task ClosDialog()
    {
        _dialog.SetVisible(false);
        return Task.CompletedTask;
    }
}
