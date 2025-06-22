using AWUI.Enums;
using AWUI.Interfaces;

namespace AWUI.Helper;

public readonly struct Error : IError
{
    public string Message { get; }

    public ErrorType Code { get; }

    public IReadOnlyCollection<IError> Errors { get; }

    public Exception? Exception { get; }

    public Error()
    {
        Message = string.Empty;
        Code = ErrorType.Unknown;
        Errors = Array.Empty<IError>();
        Exception = null;
    }

    public Error(
        string message, 
        ErrorType code = ErrorType.Unknown, 
        IReadOnlyCollection<IError>? errors = null, 
        Exception? exception = null)
    {
        Message = message;
        Code = code;
        Errors = errors ?? Array.Empty<IError>();
        Exception = exception;
    }
}

public readonly struct Error<T> : IError<T>
{
    public string Message { get; }
    public ErrorType Code { get; }
    public T Data { get; }
    public IReadOnlyCollection<IError> Errors { get; }
    public Exception? Exception { get; }

    public Error(
        string message,
        ErrorType code = ErrorType.Unknown,
        T data = default!,
        IReadOnlyCollection<IError>? errors = null,
        Exception? exception = null)
    {
        Message = message;
        Code = code;
        Data = data;
        Errors = errors ?? Array.Empty<IError>();
        Exception = exception;
    }
}

public readonly struct Result : IResult
{
    public ResultType Type { get; }

    public IError? Error { get; }

    public bool IsSuccess => Type is ResultType.Ok;

    public Result(ResultType type)
    {
        Type = type;
        Error = null;
    }

    public Result(ResultType type, IError error)
    {
        if(type == ResultType.Error && error == null)
            throw new ArgumentNullException(nameof(error), "Error cannot be null when type is Error.");

        Type = type;
        Error = type == ResultType.Error ? error : null;
    }

    public static Result Ok => new Result(ResultType.Ok);

    public static Result NotOk => Fail(new Error());

    public static Result Fail(IError error) 
            => new Result(ResultType.Error, error);

    public static Result Fail(string message, ErrorType code = ErrorType.Unknown)
            => new Result(ResultType.Error, new Error(message, code));

    public static implicit operator Result(Error error) => Fail(error);

    public static implicit operator Result(Exception ex) => FromException(ex);

    public void Deconstruct(out bool isSuccess, out IError? error)
    {
        isSuccess = IsSuccess;
        error = Error ?? (IsSuccess ? null : new Error("Unknown error"));
    }

    public static Result FromException(Exception ex, string? message = null)
        => new Result(
            ResultType.Error,
            new Error(message ?? ex.Message, ErrorType.System, exception: ex));
}

public readonly struct Result<T> : IResult<T>
{
    public T Data { get; }

    public ResultType Type { get; }

    public IError? Error { get; }

    public bool IsSuccess => Type is ResultType.Ok;

    public Result(T data)
    {
        if(data == null)
            throw new ArgumentNullException(nameof(data), "Data cannot be null.");

        Type = ResultType.Ok;
        Data = data;
        Error = null;
    }

    public Result(ResultType type, T data, IError error)
    {
        if(type == ResultType.Error && error == null)
            throw new ArgumentNullException(nameof(error), "Error cannot be null when type is Error.");

        Type = type;
        Data = data;
        Error = type == ResultType.Error ? error : null;
    }

    public static Result<T> Success(T data) => new Result<T>(data);

    public static Result<T> Fail(T data)
    {
        return new Result<T>(ResultType.Error, data, new Error<T>(string.Empty, ErrorType.Unknown, data, null));
    }

    public static Result<T> Fail(string message, T data, ErrorType code = ErrorType.Unknown, IReadOnlyCollection<IError>? errors = null)
    {
        return new Result<T>(ResultType.Error, data, new Error<T>(message, code, data, errors));    
    }

    public static Result<T> Fail(IError error) => new Result<T>(ResultType.Error, default!, error);

    public static Result<T> FromException(Exception ex, string? message = null)
    {
        T t = default(T)!;
        return new Result<T>(ResultType.Error, t, new Error(
                message ?? ex.Message,
                ErrorType.Unknown,
                exception: ex));
    }

    public static implicit operator Result<T>(T data) => Success(data);

    public static implicit operator Result<T>(Error error) => Fail(error);

    public static implicit operator Result<T>(Exception ex) => FromException(ex);

    public void Deconstruct(out bool isSuccess, out T? data, out IError? error)
    {
        isSuccess = IsSuccess;
        data = IsSuccess ? Data : default;
        error = Error ?? (IsSuccess ? null : new Error("Unknown error"));
    }
}
