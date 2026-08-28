namespace Pdv.Domain.Common;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error {  get; } = string.Empty;

    protected Result(bool success, string error)
    {
        IsSuccess = success;
        Error = error;  
    }

    public static Result Ok() => new(true, string.Empty);
    public static Result Fail(string error) => new(false, error);
}

public class Result<T> : Result
{
    public T? Value { get; }

    private Result(bool success, T? value, string error) : base(success, error)
    {
        Value = value;
    }

    public static Result<T> Ok(T value) => new(true, value, string.Empty);
    public static new Result<T> Fail(string error) => new(false, default, error);
}
