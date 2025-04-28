
using System.Diagnostics.CodeAnalysis;

namespace SharedLibrary.Services;

public static class ResultExtensions
{
    public static bool TryGetData<T>(this IResult<T> result, [MaybeNullWhen(false)] out T data)
    {
        if (result.IsSuccess && result.Data is T)
        {
            data = (T)result.Data;
            return true;
        }

        data = default(T);
        return false;
    }

    public static bool TryGetData<T>(this IError<T> error, [MaybeNullWhen(false)] out T data)
    {
        data = error.Data is T t ? t : default;
        return data is not null || (error.Data is T && error.Data == null);
    }

    public static Result<T> OnSuccess<T>(this Result<T> result, Action<T> handleResult)
    {
        if(result.IsSuccess) handleResult(result.Data);
        return result;
    }

    public static Result<T> OnFailure<T>(this Result<T> result, Action<IError> handleError)
    {
        if (!result.IsSuccess) handleError(result.Error!);
        return result;
    }
}
