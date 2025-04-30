namespace SharedLibrary.Models;

public class ViewBox
{
    public ViewBox(int minX, int minY, int width, int height)
    {
        MinX = minX;
        MinY = minY;
        Width = width;
        Height = height;
    }

    public int Width { get; set; }

    public int Height { get; set; }

    public int MinX { get; set; }

    public int MinY { get; set; }
}
