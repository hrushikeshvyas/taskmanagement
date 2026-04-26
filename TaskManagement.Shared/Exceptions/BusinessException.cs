namespace TaskManagement.Shared.Exceptions;

public class BusinessException : Exception
{
    public string Code { get; set; }
    public List<string> Errors { get; set; } = [];

    public BusinessException(string message, string code = "BUSINESS_ERROR")
        : base(message)
    {
        Code = code;
    }

    public BusinessException(string message, string code, List<string> errors)
        : base(message)
    {
        Code = code;
        Errors = errors;
    }
}