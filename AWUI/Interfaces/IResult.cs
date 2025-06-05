using AWUI.Enums;

namespace AWUI.Interfaces;

public interface IResult
{
    ResultType Type { get; }

    IError? Error { get; }

    bool IsSuccess { get; }
}

public interface IResult<out T> : IResult
{
    T Data { get; }
}
