using System.ComponentModel.DataAnnotations;
using User_Service.DTOs.Common;

namespace User_Service.Common.Utilities.Implementations;

public static class ValidationHelper
{

    public static Error[]? Validate<T>(T obj) where T : class
    {
        var context = new ValidationContext(obj);

        var results = new List<ValidationResult>();

        if (Validator.TryValidateObject(
            obj,
            context,
            results,
            true))
        {
            return null;
        }

        return results
            .Select(x =>
                Error.Validation(
                    code: x.MemberNames.FirstOrDefault() ?? "ValidationError",
                    description: x.ErrorMessage ?? "Validation failed"
                ))
            .ToArray();
    }
}