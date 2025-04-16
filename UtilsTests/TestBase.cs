using Xunit.Abstractions;

namespace SharedLibraryTests;

public abstract class TestBase
{
    protected readonly ITestOutputHelper _output;

    protected TestBase(ITestOutputHelper testOutputHelper)
    {
        _output = testOutputHelper;
    }

    protected virtual void LogAssertEqual<T>(string message, T actual, T expected)
    {
        _output.WriteLine($"{message}: Expected={expected}, Actual={actual}");
        Assert.Equal(expected, actual);
    }
}
