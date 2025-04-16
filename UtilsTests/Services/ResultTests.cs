using SharedLibrary.Services;
using Xunit.Abstractions;

namespace SharedLibraryTests.ServicesTests;

public class ResultTests : TestBase
{
    public ResultTests(ITestOutputHelper output) : base(output) {}

    [Fact]
    public void TestResultType()
    {
        LogAssertEqual("ResultType.Ok", (byte)ResultType.Ok, 0);
        LogAssertEqual("ResultType.Error", (byte)ResultType.Error, 1);
    }

    [Fact]
    public void TestErrorType()
    {
        LogAssertEqual("ErrorType.Unknown", (byte)ErrorType.Unknown, 0);
        LogAssertEqual("ErrorType.Custom", (byte)ErrorType.Custom, 1);
        LogAssertEqual("ErrorType.System", (byte)ErrorType.System, 2);
    }

    [Fact]
    public void TestCreateResult()
    {
        var error = new Error("Test error", ErrorType.Unknown);
        var result = Result.Fail(error);

        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);

        _output.WriteLine($"Error Message : {result.Error?.Message}");
    }


}
