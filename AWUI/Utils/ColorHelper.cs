using AWUI.Enums;

namespace AWUI.Utils;

public static class ColorHelper
{
    public static string ConvertToString(ColorType colorType)
    {
        var colorStyle = colorType switch
        {
            ColorType.Blue => "blue",
            ColorType.Red => "red",
            ColorType.Green => "green",
            ColorType.Yellow => "yellow",
            ColorType.Orange => "orange",
            ColorType.Purple => "purple",
            ColorType.Black => "black",
            ColorType.White => "white",
            ColorType.Gray => "gray",
            ColorType.Brown => "brown",
            ColorType.Cyan => "cyan",
            ColorType.Magenta => "magenta",
            _ => "Black" // Default color
        };

        return colorStyle;
    }
}
