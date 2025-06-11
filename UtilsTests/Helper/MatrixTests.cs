using AWUI.Helper;
using Xunit.Abstractions;

namespace SharedLibraryTests.Helper;

public class MatrixTests : TestBase
{
    public MatrixTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public void TestMatrixConstructor_1()
    {
        // Arrange
        var data = new double[6] { 1.1, 2.2, 3.3, 4.4, 5.5, 6.6 };

        // Act
        var matrix = Matrix.From2DArray(data, 2, 3);

        // Assert
        Assert.NotNull(matrix);
        Assert.Equal(data, matrix.RowFlatten());
    }
}
