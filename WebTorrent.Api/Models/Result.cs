namespace WebTorrent.Api.Models;
public enum ResultStatusCode
{
    None,
    ValidationError,
    NotFound,
    Error
    // to be enhanced, add the status code you need (based on the logic you implement (expected errors to handle) in your handler)
}

public class Result<T>
{
    public bool IsSuccessful { get; }
    public ResultStatusCode StatusCode { get; }
    public List<string> ErrorMessages { get; }
    public T Data { get; }

    internal Result(bool isSuccessful, ResultStatusCode statusCode, List<string> errorMessages, T data)
    {
        IsSuccessful = isSuccessful;
        StatusCode = statusCode;
        ErrorMessages = errorMessages;
        Data = data;
    }

    public static Result<T> Success(T data)
    {
        return new Result<T>(true, ResultStatusCode.None, default!, data);
    }

    public static Result<T> Failure(ResultStatusCode statusCode, string errorMessage)
    {
        return new Result<T>(false, statusCode, new List<string>() { errorMessage }, default!);
    }

    public static Result<T> Failure(ResultStatusCode statusCode, List<string> errorMessages)
    {
        return new Result<T>(false, statusCode, errorMessages, default!);
    }
}

public class Result : Result<object>
{
    private Result(bool isSuccessful, ResultStatusCode statusCode, List<string> errorMessages)
        : base(isSuccessful, statusCode, errorMessages, null!)
    {
    }

    public static Result Success()
    {
        return new Result(true, ResultStatusCode.None, new List<string>());
    }

    public static new Result Failure(ResultStatusCode statusCode, string errorMessage)
    {
        return new Result(false, statusCode, new List<string> { errorMessage });
    }

    public static new Result Failure(ResultStatusCode statusCode, List<string> errorMessages)
    {
        return new Result(false, statusCode, errorMessages);
    }
}