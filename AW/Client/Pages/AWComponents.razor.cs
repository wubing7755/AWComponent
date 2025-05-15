using AWUI.Components;
using AWUI.JsInterop;
using AWUI.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;

namespace AW.Client.Pages;

public partial class AWComponents
{
    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    public AWJsInterop AWJsInterop { get; set; }

    private HubConnection _hubConnection { get; set; }
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

    private FileMetaData _upFileInfo;

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
        await AWJsInterop.TestConnection();
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
        var url = $"{NavigationManager.BaseUri}api/upload/chunk";
        await AWJsInterop.UploadFileAsync(_fileInputRef, url);
    }

    private async Task UpLoadFiles(MouseEventArgs args)
    {
        var url = $"{NavigationManager.BaseUri}api/upload/chunk";
        await AWJsInterop.UploadFilesAsync(_filesInputRef, url);
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
