namespace LouisManager.Api.Exceptions;

public class BusinessValidationException : Exception
{
    public List<string> Errors { get; }

    public BusinessValidationException(List<string> errors) : base("There are multiple errors.")
    {
        Errors = errors;
    }

    public BusinessValidationException(string message, List<string> errors) : base(message)
    {
        Errors = errors;
    }

    public BusinessValidationException(string message, Exception inner, List<string> errors) : base(message, inner)
    {
        Errors = errors;
    }
}