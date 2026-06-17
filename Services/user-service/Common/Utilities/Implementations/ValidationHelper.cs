using System.ComponentModel.DataAnnotations;

namespace User_Service.Common.Utilities.Implementations;

public static class ValidationHelper
{
    public static IResult? HandleValidation<T>(T obj) where T : class
    {
        var errors = new List<string>();

        var context = new ValidationContext(obj);

        var results = new List<ValidationResult>();

        if (!Validator.TryValidateObject(
            obj,
            context,
            results,
            true))
        {
            return Results.ValidationProblem(
                results.ToDictionary(
                    x => x.MemberNames.First(),
                    x => new[] { x.ErrorMessage! }
                )
            );
        }

        return null;
    }
}