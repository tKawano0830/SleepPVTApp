namespace SleepPvtTracker.Core.Common;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string ErrorMessage { get; }

    protected Result(bool isSuccess, string errorMessage)
    {
        if (isSuccess && !string.IsNullOrEmpty(errorMessage))
            throw new InvalidOperationException("成功時にエラーメッセージは設定できません");
        if (!isSuccess && string.IsNullOrEmpty(errorMessage))
            throw new InvalidOperationException("失敗時にはエラーメッセージが必要です");

        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public static Result Ok() => new Result(true, string.Empty);
    public static Result Fail(string errorMessage) => new Result(false, errorMessage);
}

public class Result<T> : Result
{
    private readonly T? _value;
    public T Value => IsSuccess ? _value! : throw new InvalidOperationException("失敗したResultから値を取得することはできません");

    private Result(bool isSuccess, string errorMessage, T? value) : base(isSuccess, errorMessage)
    {
        _value = value;
    }

    public static Result<T> Ok(T value) => new Result<T>(true, string.Empty, value);
    public static new Result<T> Fail(string errorMessage) => new Result<T>(false, errorMessage, default);
}