using System.Diagnostics.CodeAnalysis;

namespace SharedLibrary.Services;

#region EnumType

/// <summary>
/// Result 类型
/// </summary>
public enum ResultType
{
    /// <summary> 操作成功 </summary>
    Ok,
    /// <summary> 操作失败 </summary>
    Error
}

/// <summary>
/// Error 类型
/// </summary>
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

public interface IError
{
    string Message { get; }

    ErrorType Code { get; }
}

public interface IResult
{
    ResultType Type { get; }

    IError? Error { get; }

    bool IsSuccess { get; }
}

public interface IResult<T> : IResult
{
    T? Data { get; }

    bool TryGetData([MaybeNullWhen(false)] out T data);
}

#endregion

#region Implement

public class Error : IError
{
    public string Message { get; }

    public ErrorType Code { get; }

    public object? Data { get; }

    public Error(string message, ErrorType code = ErrorType.Unknown, object? data = null)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Code = code;
        Data = data;
    }

    public bool TryGetData<T>([MaybeNullWhen(false)] out T data)
    {
        //if (Data is T)
        //{
        //    data = (T)Data;
        //    return true;
        //}

        //data = default(T);
        //return false;
        data = Data is T t ? t : default;
        return data != null || (Data is T && Data == null);
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


    public bool TryGetData([MaybeNullWhen(false)] out T data)
    {
        if(IsSuccess)
        {
            if (Data is T)
            {
                data = (T)Data;
                return true;
            }
        }

        data = default(T);
        return false;
    }
}

#endregion