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

    #region Basic Tests

    [Fact]
    public void ResultType_Values_ShouldBeCorrect()
    {
        Assert.Equal(0, (int)ResultType.Ok);
        Assert.Equal(1, (int)ResultType.Error);
    }

    [Fact]
    public void ErrorType_Values_ShouldBeCorrect()
    {
        Assert.Equal(0, (int)ErrorType.Unknown);
        Assert.Equal(1, (int)ErrorType.Custom);
        Assert.Equal(2, (int)ErrorType.System);
    }

    #endregion

    #region Result Tests

    [Fact]
    public void Success_Result_ShouldHaveCorrectProperties()
    {
        // Act
        var result = Result.Success();
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(ResultType.Ok, result.Type);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Fail_WithError_ShouldHaveCorrectProperties()
    {
        // Arrange
        var error = new Error("Test error");
        // Act
        var result = Result.Fail(error);
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultType.Error, result.Type);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void Fail_WithMessage_ShouldCreateError()
    {
        // Arrange
        const string errorMessage = "Test error message";
        // Act
        var result = Result.Fail(errorMessage);
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultType.Error, result.Type);
        Assert.Equal(errorMessage, result.Error?.Message);
        Assert.Equal(ErrorType.Unknown, result.Error?.Code);
    }

    [Fact]
    public void Fail_WithException_ShouldCreateErrorWithException()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        // Act
        var result = Result.FromException(exception);
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultType.Error, result.Type);
        Assert.Equal(exception.Message, result.Error?.Message);
        Assert.Equal(ErrorType.System, result.Error?.Code);
        Assert.Equal(exception, result.Error?.Exception);
    }

    [Fact]
    public void FromException_WithCustomMessage_ShouldUseCustomMessage()
    {
        // Arrange
        var exception = new Exception("Original message");
        const string customMessage = "Custom message";
        // Act
        var result = Result.FromException(exception, customMessage);
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(customMessage, result.Error?.Message);
        Assert.Equal(ErrorType.System, result.Error?.Code);
        Assert.Equal(exception, result.Error?.Exception);
    }

    [Fact]
    public void Deconstruct_ShouldReturnCorrectValues()
    {
        // Arrange
        var successResult = Result.Success();
        var error = new Error("Test error");
        var failResult = Result.Fail(error);
        // Act
        var (success1, error1) = successResult;
        var (success2, error2) = failResult;
        // Assert
        Assert.True(success1);
        Assert.Null(error1);
        Assert.False(success2);
        Assert.Equal(error, error2);
    }

    [Fact]
    public void ImplicitConversion_FromError_ShouldCreateFailedResult()
    {
        // Arrange
        var error = new Error("Test error");
        // Act
        Result result = error;
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void ImplicitConversion_FromException_ShouldCreateFailedResult()
    {
        // Arrange
        var exception = new Exception("Test exception");
        // Act
        Result result = exception;
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(exception.Message, result.Error?.Message);
        Assert.Equal(ErrorType.System, result.Error?.Code);
        Assert.Equal(exception, result.Error?.Exception);
    }

    #endregion

    #region Result<T> Tests

    [Fact]
    public void Success_WithData_ShouldHaveCorrectProperties()
    {
        // Arrange
        const string testData = "Test data";
        // Act
        var result = Result<string>.Success(testData);
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(ResultType.Ok, result.Type);
        Assert.Null(result.Error);
        Assert.Equal(testData, result.Data);
    }

    [Fact]
    public void Success_WithNullData_ShouldThrow()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => Result<string>.Success(null!));
    }

    [Fact]
    public void Fail_WithDataAndError_ShouldHaveCorrectProperties()
    {
        // Arrange
        const string testData = "Test data";
        var error = new Error<string>("Test error", data: testData);
        // Act
        var result = Result<string>.Fail(error);
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultType.Error, result.Type);
        Assert.Equal(error, result.Error);
        Assert.Equal(testData, ((IError<string>)result.Error!).Data);
    }

    [Fact]
    public void Fail_WithDataAndMessage_ShouldCreateErrorWithData()
    {
        // Arrange
        const string testData = "Test data";
        const string errorMessage = "Test error message";
        // Act
        var result = Result<string>.Fail(errorMessage, testData);
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultType.Error, result.Type);
        Assert.Equal(errorMessage, result.Error?.Message);
        Assert.Equal(ErrorType.Unknown, result.Error?.Code);
        Assert.Equal(testData, ((IError<string>)result.Error!).Data);
        Assert.Equal(testData, result.Data);
    }

    [Fact]
    public void FromException_WithData_ShouldCreateErrorWithDefaultData()
    {
        // Arrange
        var exception = new Exception("Test exception");
        // Act
        var result = Result<string>.FromException(exception);
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(exception.Message, result.Error?.Message);
        Assert.Equal(ErrorType.System, result.Error?.Code);
        Assert.Equal(exception, result.Error?.Exception);
        Assert.Null(result.Data);
    }

    [Fact]
    public void Deconstruct_WithData_ShouldReturnCorrectValues()
    {
        // Arrange
        const string successData = "Success data";
        const string failData = "Fail data";
        var error = new Error<string>("Test error", data: failData);

        var successResult = Result<string>.Success(successData);
        var failResult = Result<string>.Fail(error);
        // Act
        var (success1, data1, error1) = successResult;
        var (success2, data2, error2) = failResult;
        // Assert
        Assert.True(success1);
        Assert.Equal(successData, data1);
        Assert.Null(error1);

        Assert.False(success2);
        Assert.Equal(failData, data2);
        Assert.Equal(error, error2);
    }

    [Fact]
    public void ImplicitConversion_FromData_ShouldCreateSuccessResult()
    {
        // Arrange
        const string testData = "Test data";
        // Act
        Result<string> result = testData;
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(testData, result.Data);
    }

    [Fact]
    public void ImplicitConversion_FromError_ShouldCreateFailedResultWithDefaultData()
    {
        // Arrange
        var error = new Error("Test error");
        // Act
        Result<string> result = error;
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);
        Assert.Null(result.Data);
    }

    [Fact]
    public void ImplicitConversion_FromException_ShouldCreateFailedResultWithDefaultData()
    {
        // Arrange
        var exception = new Exception("Test exception");
        // Act
        Result<string> result = exception;
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(exception.Message, result.Error?.Message);
        Assert.Equal(ErrorType.System, result.Error?.Code);
        Assert.Equal(exception, result.Error?.Exception);
        Assert.Null(result.Data);
    }

    #endregion

    #region Error Tests

    [Fact]
    public void Error_Constructor_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        const string message = "Test message";
        var errors = new List<IError> { new Error("Nested error") };
        var exception = new Exception("Test exception");
        // Act
        var error = new Error(message, ErrorType.Custom, errors, exception);
        // Assert
        Assert.Equal(message, error.Message);
        Assert.Equal(ErrorType.Custom, error.Code);
        Assert.Equal(errors, error.Errors);
        Assert.Equal(exception, error.Exception);
    }

    [Fact]
    public void Error_WithNullMessage_ShouldThrow()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Error(null!));
    }

    [Fact]
    public void ErrorT_Constructor_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        const string message = "Test message";
        const int data = 42;
        var errors = new List<IError> { new Error("Nested error") };
        var exception = new Exception("Test exception");
        // Act
        var error = new Error<int>(message, ErrorType.Custom, data, errors, exception);
        // Assert
        Assert.Equal(message, error.Message);
        Assert.Equal(ErrorType.Custom, error.Code);
        Assert.Equal(data, error.Data);
        Assert.Equal(errors, error.Errors);
        Assert.Equal(exception, error.Exception);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Fail_WithNullError_ShouldThrow()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => Result.Fail(null!));
    }

    [Fact]
    public void ResultT_Fail_WithNullError_ShouldThrow()
    {
        string str = string.Empty;

        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => Result<string>.Fail(str));
    }

    [Fact]
    public void Result_Constructor_WithErrorTypeButNoError_ShouldThrow()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Result(ResultType.Error, null!));
    }

    [Fact]
    public void ResultT_Constructor_WithErrorTypeButNoError_ShouldThrow()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Result<string>(ResultType.Error, "data", null!));
    }

    #endregion
}
