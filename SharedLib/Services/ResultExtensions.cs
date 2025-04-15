
namespace SharedLibrary.Services;

public static class ResultExtensions
{
    public static Result<T> Ok<T>(T data) => new(ResultType.Ok, data);
    public static Result<T> Error<T>(IError error) => new(ResultType.Error, error: error);
}
