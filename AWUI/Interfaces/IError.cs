using AWUI.Enums;

namespace AWUI.Interfaces;

public interface IError
{
    string Message { get; }

    ErrorType Code { get; }

    IReadOnlyCollection<IError> Errors { get; }

    Exception? Exception { get; }
}

public interface IError<out T> : IError
{
    T Data { get; }
}
