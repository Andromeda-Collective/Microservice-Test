
using Microsoft.AspNetCore.Mvc;
using User_Service.DTOs.Common;

namespace User_Service.Common.Utilities.Implementations;

public static class ErrorResponse
{
    private static ProblemDetails CreateProblemDetails(
    string title,
    int status,
    Error error,
    Error[]? errors = null
    ) => new()
    {
        Title = title,
        Type = error.Code,
        Detail = error.Description,
        Status = status,
        Extensions = { { nameof(errors), errors } },
    };


    public static IResult HandleFailure(Error error, Error[]? errors = null) =>
    error switch
    {
        { Type: ErrorType.Validation } => Results.BadRequest(CreateProblemDetails(
            "Validation Error",
            StatusCodes.Status400BadRequest,
            error,
            errors
        )),
        { Type: ErrorType.NotFound } => Results.NotFound(CreateProblemDetails(
            "Not Found",
            StatusCodes.Status404NotFound,
            error,
            errors
        )),
        { Type: ErrorType.Conflict } => Results.Conflict(CreateProblemDetails(
            "Conflict",
            StatusCodes.Status409Conflict,
            error,
            errors
        )),
        _ => Results.BadRequest(CreateProblemDetails(
            "Bad Request",
            StatusCodes.Status400BadRequest,
            error
        ))
    };
}