using AWUI.Utils;
using Xunit.Abstractions;

namespace SharedLibraryTests.Utils;

public class MatrixHelperTests : TestBase
{
    public MatrixHelperTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}

    [Fact]
    public void TestMatrix2x2()
    {
        var matrix2x2 = Matrix2x2.Identity;

        _output.WriteLine($"matrix2x2 = {matrix2x2.ToString()}");

        _output.WriteLine($"inverse = {matrix2x2.Inverse().ToString()}");

        _output.WriteLine($"determinant = {matrix2x2.Determinant()}");
    }
}
