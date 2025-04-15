using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

namespace SharedLibrary.Services;

#region EnumType

/// <summary> Result 类型 </summary>
public enum ResultType
{
    /// <summary> 操作成功 </summary>
    Ok,
    /// <summary> 操作失败 </summary>
    Error
}

/// <summary> Error 类型 </summary>
public enum ErrorType
{
    /// <summary> 未知错误 </summary>
    Unknown,
    /// <summary> 系统错误 </summary>
    System,
    /// <summary> 一般性错误 </summary>
    Custom,
}

#endregion

#region Interface

public interface IResult
{
    ResultType Type { get; }

    IError? Error { get; }

    bool IsSuccess { get; }
}

public interface IResult<out T> : IResult
{
    T? Data { get; }
}

public interface IError
{
    string Message { get; }

    ErrorType Code { get; }

    IReadOnlyCollection<Error> Errors { get; }

    Exception? Exception { get; }
}

public interface IError<out T> : IError
{
    T? Data { get; }
}

#endregion

#region Implement

public readonly struct Error : IError
{
    public string Message { get; }

    public ErrorType Code { get; }

    public IReadOnlyCollection<Error> Errors { get; }

    public Exception? Exception { get; }

    public Error(
        string message, 
        ErrorType code = ErrorType.Unknown, 
        IReadOnlyCollection<Error>? errors = null, 
        Exception? exception = null)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Code = code;
        Errors = errors ?? Array.Empty<Error>();
        Exception = exception;
    }
}


public readonly struct Error<T> : IError<T>
{
    public string Message { get; }
    public ErrorType Code { get; }
    public T? Data { get; }
    public IReadOnlyCollection<Error> Errors { get; }
    public Exception? Exception { get; }

    public Error(
        string message,
        ErrorType code = ErrorType.Unknown,
        T? data = default,
        IReadOnlyCollection<Error>? errors = null,
        Exception? exception = null)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Code = code;
        Data = data;
        Errors = errors ?? Array.Empty<Error>();
        Exception = exception;
    }
}


public readonly struct Result : IResult
{
    public ResultType Type { get; }

    public IError? Error { get; }

    public bool IsSuccess => Type is ResultType.Ok;

    public Result(ResultType type, IError? error = null)
    {
        Type = type;
        Error = type == ResultType.Error ? error : null;
    }

    public static Result Success() => new Result(ResultType.Ok);

    public static Result Fail() => new Result(ResultType.Error);

    public static Result Fail(string message, ErrorType code = ErrorType.Unknown, object? data = null, IReadOnlyCollection<Error>? errors = null)
    {
        return new Result(ResultType.Error, new Error(message, code, data, errors));
    }

    public static Result Fail(IError error)
    {
        return new Result(ResultType.Error, error);
    }

    public static implicit operator Result(Error error) => Fail(error);

    public void Deconstruct(out bool isSuccess, out IError? error)
    {
        isSuccess = IsSuccess;
        error = Error;
    }
}

public readonly struct Result<T> : IResult<T>
{
    public T? Data { get; }

    public ResultType Type { get; }

    public IError? Error { get; }

    public bool IsSuccess => Type is ResultType.Ok;

    public Result(ResultType type, T? data = default, IError? error = null)
    {
        Type = type;
        Data = data;
        Error = type == ResultType.Error ? error : null;
    }

    public static Result<T> Success(T data) => new Result<T>(ResultType.Ok, data, null);

    public static Result<T> Fail(T data)
    {
        return new Result<T>(ResultType.Error, default, new Error<T>(string.Empty, ErrorType.Unknown, data, null));
    }

    public static Result<T> Fail(string message, T data, ErrorType code = ErrorType.Unknown, IReadOnlyCollection<Error>? errors = null)
    {
        return new Result<T>(ResultType.Error, default, new Error<T>(message, code, data, errors));    
    }

    public static Result<T> Fail(IError error) => new Result<T>(ResultType.Error, default, error);

    public static Result FromException(Exception ex, string? message = null)
    => new(ResultType.Error, new Error(
        message ?? ex.Message,
        ErrorType.System,
        exception: ex));

    public static implicit operator Result<T>(T data) => Success(data);

    public static implicit operator Result<T>(Error error) => Fail(error);

    public void Deconstruct(out bool isSuccess, out T? data, out IError? error)
    {
        isSuccess = IsSuccess;
        data = Data;
        error = Error;
    }
}

#endregion