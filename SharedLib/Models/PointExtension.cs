namespace SharedLibrary.Models;

public static class PointExtension
{
    public static Point SetPointXY(this Point point, double x, double y)
    {
        point.X = x;
        point.Y = y;
        return point;
    }
}
