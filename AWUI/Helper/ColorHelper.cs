using AWUI.Enums;

namespace AWUI.Helper;

public static class ColorHelper
{
    public static string ConvertToString(ColorType colorType)
    {
        var colorStyle = colorType switch
        {
            ColorType.Blue => $"rgba(33, 150, 243)",    // Blue 500
            ColorType.Red => $"rgba(244, 67, 54)",      // Red 500
            ColorType.Green => $"rgba(76, 175, 80)",    // Green 500
            ColorType.Yellow => $"rgba(255, 235, 59)",  // Yellow 500
            ColorType.Orange => $"rgba(255, 152, 0)",   // Orange 500
            ColorType.Purple => $"rgba(156, 39, 176)",  // Purple 500
            ColorType.Black => $"rgba(33, 33, 33)",     // Dark Grey 
            ColorType.White => $"rgba(255, 255, 255)",  // White
            ColorType.Grey => $"rgba(158, 158, 158)",   // Material Grey 500
            ColorType.Brown => $"rgba(121, 85, 72)",    // Material Brown 500
            ColorType.Cyan => $"rgba(0, 188, 212)",     // Material Cyan 500
            ColorType.Magenta => $"rgba(233, 30, 99)",  // Material Pink 500
            _ => $"rgba(255, 255, 255)"                 // None
        };

        return colorStyle;
    }
}
