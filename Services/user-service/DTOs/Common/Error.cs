namespace User_Service.DTOs.Common;

public sealed class Error
{
    public Error(string code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;

    }
    public string Code { get; }
    public string Description { get; }
    public ErrorType Type { get; }


    public static Error Validation(string code, string description) => new Error(code, description, ErrorType.Validation);
    public static Error Validation() => new Error("ValidationError", "Some fields are not correct", ErrorType.Validation);
    public static Error NotFount(string code, string description) => new Error(code, description, ErrorType.NotFound);
    public static Error Conflict(string code, string description) => new Error(code, description, ErrorType.Conflict);
    public static Error NullValue(string code) => new Error(code, $"The value can not be null/empty", ErrorType.Validation);

}