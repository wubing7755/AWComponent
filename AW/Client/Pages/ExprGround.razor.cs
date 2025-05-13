using AWUI.Models;
using AWUI.Utils;
using Microsoft.AspNetCore.Components.Web;

namespace AW.Client.Pages;

public partial class ExprGround
{
    private string _inputext = string.Empty;
    private string _translationText = string.Empty;
    private const string DividerStyle = "display: block; height: 30px; color:green;";

    private async Task ConvertToXML(MouseEventArgs args)
    {
        var xmlWriter = new XmlWriter(awJsInterop);

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
}
