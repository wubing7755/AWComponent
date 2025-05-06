using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using SharedLibrary.Enums;
using SharedLibrary.Models;
using SharedLibrary.Utils;

namespace SharedLibrary.Components;

/**
 * 拖动画布上元素：
 *      
 */
public class Diagram : AWComponentBase
{
    /// <summary>
    /// 可视区域
    /// </summary>
    [Parameter]
    public ViewBox ViewBox { get; set; } = new ViewBox(0, 0, 512, 512);

    /// <summary>
    /// 物理尺寸-宽度
    /// </summary>
    [Parameter]
    public double Width { get; set; } = 512;

    /// <summary>
    /// 物理尺寸-高度
    /// </summary>
    /// <remarks>
    /// 默认高度是 100%
    /// </remarks>
    [Parameter]
    public double Height { get; set; } = 512;

    [Parameter]
    public ColorType ColorType { get; set; } = ColorType.Black;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// 显式坐标轴
    /// </summary>
    [Parameter] 
    public bool ShowAxis { get; set; } = true;

    [Parameter] 
    public string AxisColor { get; set; } = "red";

    [Parameter] 
    public string AxisDashArray { get; set; } = "30.9 5";

    [Parameter]
    public Func<MouseEventArgs, Task>? OnClick { get; set; }

    private List<DraggableSVGElement> Elements { get; } = new();

    private readonly float Version = 1.1f;

    protected override void BuildComponent(RenderTreeBuilder builder)
    {
        int seq = 0;

        builder.OpenElement(seq++, "div");
        builder.AddAttribute(seq++, "tabindex", "0");
        // HandleKeyDown
        builder.AddAttribute(seq++, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, async (args) =>
        {
            await HandleKeyDown(args);
        }));

        builder.OpenElement(seq++, "svg");
        builder.AddAttribute(seq++, "xmlns", "http://www.w3.org/2000/svg");
        builder.AddAttribute(seq++, "version", $"{Version}");
        builder.AddAttribute(seq++, "viewBox", $"{ViewBox.MinX} {ViewBox.MinY} {ViewBox.Width} {ViewBox.Height}");

        /**
        * "[align] [meetOrSlice]"
        * align： viewBox在视口中的对齐方式
        * meetOrSlice： 如何适应宽高比，默认为meet
        */
        builder.AddAttribute(seq++, "preserveAspectRatio", $"{nameof(SvgAlign.xMidYMid)} {nameof(SvgMeetOrSlice.meet)}");
        builder.AddAttribute(seq++, "width", Width);
        builder.AddAttribute(seq++, "height", Height);
        builder.AddAttribute(seq++, "style", $"background-color: {ColorHelper.ConvertToString(ColorType)};");

        /* 翻转坐标系，与笛卡尔坐标系保持一致 */
        builder.OpenElement(seq++, "g");
        builder.AddAttribute(seq++, "transform", $"matrix(1 0 0 -1 {Width/2} {Height/2})");

        /* 原点 */
        builder.OpenElement(seq++, "circle");
        builder.AddMultipleAttributes(seq++, new Dictionary<string, object>
        {
            {"cx", "0" },
            {"cy", "0" },
            {"r", "5" },
            {"fill", "red" }
        });
        builder.CloseElement();

        /* X轴 */
        builder.OpenElement(seq++, "line");
        builder.AddMultipleAttributes(seq++, new Dictionary<string, object>
        {
            {"x1", "-256" },
            {"y1", "0" },
            {"x2", "256" },
            {"y2", "0" },
            {"stroke", AxisColor },
            {"stroke-width", "1" },
            {"stroke-dasharray", AxisDashArray }
        });
        builder.CloseElement();

        /* Y轴 */
        builder.OpenElement(seq++, "line");
        builder.AddMultipleAttributes(seq++, new Dictionary<string, object>
        {
            {"x1", "0" },
            {"y1", "-256" },
            {"x2", "0" },
            {"y2", "256" },
            {"stroke", AxisColor },
            {"stroke-width", "1" },
            {"stroke-dasharray", AxisDashArray }
        });
        builder.CloseElement();

        builder.OpenComponent<CascadingValue<Diagram>>(seq++);
        builder.AddAttribute(seq++, "Value", this);
        builder.AddAttribute(seq++, "ChildContent", (RenderFragment)(childBuilder =>
        {
            childBuilder.AddContent(seq, ChildContent);
        }));
        builder.CloseComponent();

        builder.CloseElement();
        builder.CloseElement();
        builder.CloseElement();
    }

    public void AddSVGElement(DraggableSVGElement element)
    {
        Elements.Add(element);
    }

    private async Task HandleKeyDown(KeyboardEventArgs args)
    {
        Console.WriteLine($"Key pressed: {args.Key}, Code: {args.Code}");

        // 处理方向键
        switch (args.Code)
        {
            case "Delete":
                for (int i = Elements.Count - 1; i >= 0; i--)
                {
                    if (Elements[i].IsSelected)
                    {
                        Elements[i].IsDeleted = true;
                        Elements.RemoveAt(i);
                        ForceImmediateRender();
                    }
                }
                break;
        }

        await Task.CompletedTask;
    }
}
