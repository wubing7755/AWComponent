using System.Collections;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace AWUI.Components;

public class Table<TType> : AWComponentBase where TType : ICollection
{
    [Parameter]
    public TType Data { get; set; }
    
    [Parameter]
    public RenderFragment Caption { get; set; }
    
    [Parameter]
    public bool IsFixedHeader { get; set; }

    protected sealed override string BaseClass => "aw-table";

    protected override void BuildComponent(RenderTreeBuilder builder)
    {
        // table
        builder.OpenElement(0, "table");
        builder.AddAttribute(1, "class", ComputedClass + " " + $"{(IsFixedHeader ? "fixed-header" : "")}");
        builder.AddAttribute(2, "style", ComputedStyle);
        builder.AddAttribute(3, "role", "table");
        
        // caption
        builder.OpenElement(4, "caption");
        builder.AddContent(5, Caption);
        builder.CloseElement();
        
        // thead
        builder.OpenRegion(6);
        builder.OpenElement(0, "thead");
        builder.OpenElement(1, "tr");

        int index = 1;
        foreach (var item in Data)
        {
            builder.OpenElement(index++, "th");
            builder.AddContent(index++, (string)item);
            builder.CloseElement();
        }
        
        builder.CloseElement();
        builder.CloseElement();
        builder.CloseRegion();
        
        // tbody
        builder.OpenRegion(7);
        builder.OpenElement(0, "tbody");

        index = 0;
        for (int i = 0; i < 20; i++)
        {
            builder.OpenElement(index++, "tr");
            for (int j = 0; j < 5; j++)
            {
                builder.OpenElement(index++, "td");
                builder.AddContent(index++, "txt");
                builder.CloseElement();
            }
            builder.CloseElement();
        }
        
        builder.CloseElement();
        builder.CloseRegion();
        
        builder.CloseElement();
    }
}
