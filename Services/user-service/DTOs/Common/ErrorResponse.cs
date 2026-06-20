
namespace User_Service.DTOs.Common;

public static class ErrorResponse
{
    public static IResult BadRequest(
        string code,
        string description,
        HttpContext http)
    {
        return Results.BadRequest(new
        {
            type = "Validation Error",
            title = "Validation Error",
            status = 400,
            detail = description,
            errors = new[]
            {
                new {
                    code,
                    description,
                    type="Validation"
                }
            },
            requestId = http.Request.Headers["X-Request-ID"]
        });
    }
}