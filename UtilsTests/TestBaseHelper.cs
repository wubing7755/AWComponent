using Xunit.Abstractions;

namespace SharedLibraryTests;

public class TestBaseHelper
{
    private readonly ITestOutputHelper _output;

    public TestBaseHelper(ITestOutputHelper testOutputHelper)
    {
        _output = testOutputHelper;
    }

    public virtual void LogAssertEqual<T>(string message, T actual, T expected)
    {
        _output.WriteLine($"{message}: Expected={expected}, Actual={actual}");
        Assert.Equal(expected, actual);
    }
}
