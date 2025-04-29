using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using SharedLibrary.Models;
using Microsoft.JSInterop;

namespace SharedLibrary.Components;

/**
 * 功能：
 * ① 单/多文件选择
 * ② 文件类型过滤
 * ③ 文件大小限制
 * ④ 文件信息读取
 * ⑤ 事件回调
 * ⑥ 流式读取
 */
public class FileInput<TValue>: Input<TValue> where TValue : UpFileInfo, new()
{
    [Parameter]
    public bool Multiple { get; set; } = false;

    [Parameter]
    public MIMEType MIMEType { get; set; }

    [Parameter]
    public EventCallback<IEnumerable<TValue>> MultipleValueChanged { get; set; }

    protected override void BuildComponentAttributes(RenderTreeBuilder builder, ref int seq)
    {
        builder.AddAttribute(seq++, "type", InputType.File);
        builder.AddAttribute(seq++, "multiple", Multiple);

        #region Accept Type

        var acceptValue = MIMEType switch
        {
            MIMEType.Archive => ".zip,.rar,.7z",
            MIMEType.Text => ".txt,.csv,.json,.xml",
            MIMEType.PDF => ".pdf",
            _ => null
        };

        if (acceptValue != null)
        {
            builder.AddAttribute(seq++, "accept", acceptValue);
        }

        #endregion

        builder.AddAttribute(seq, "onchange", EventCallback.Factory.Create<ChangeEventArgs>
            (this, async () => await HandleFileChange()));
    }

    private async Task HandleFileChange()
    {
        try
        {
            if (!Multiple)
            {
                var file = await JsInterop.GetLocalFile(inputElement);

                if(file is not null)
                {
                    await ValueChanged.InvokeAsync(MapToFileInfo(file));
                }
            }
            else
            {
                var files = await JsInterop.GetLocalFiles(inputElement);
                var values = new List<TValue>();
                if(files is not null)
                {
                    foreach (var file in files)
                    {
                        values.Add(MapToFileInfo(file));
                    }

                    await MultipleValueChanged.InvokeAsync(values);
                }
            }

        }
        catch (JSException ex)
        {
            Console.WriteLine($"JS错误: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"文件处理错误：{ex.Message}");
        }
    }

    private static TValue MapToFileInfo(UpFileInfo file)
    {
        var cloned = file.Clone();
        return (TValue)(object)cloned;
    }
}
