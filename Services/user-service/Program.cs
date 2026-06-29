using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using User_Service.Common.Utilities.Implementations;
using User_Service.Common.Utilities.Interfaces;
using User_Service.Data;
using User_Service.DTOs.AuthDTOs;
using User_Service.DTOs.Common;
using User_Service.Services.Implementations;
using User_Service.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddSingleton<ITokenHelper, TokenHelper>();


var jwtConfig = builder.Configuration.GetSection("JWTConfig");
var secretKey = jwtConfig["SecretKey"]!;
var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidAudience = jwtConfig["Audience"],
            ValidateAudience = true,
            ValidIssuer = jwtConfig["Issuer"],
            ValidateIssuer = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
        };
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
}


app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    if (!context.Request.Headers.ContainsKey("X-Request-ID"))
    {
        context.Request.Headers["X-Request-ID"] =
            Guid.NewGuid().ToString();
    }

    context.Response.Headers["X-Request-ID"] =
        context.Request.Headers["X-Request-ID"];

    await next();
});


app.MapPost("/auth/login", async ([FromBody] AuthLoginDto user, [FromServices] IUserService userService, HttpContext http) =>
{
    var validationResult = ValidationHelper.Validate(user);
    if (validationResult is not null)
        return ErrorResponse.HandleFailure(Error.Validation(), validationResult);


    if (http.User?.Identity?.IsAuthenticated ?? false)
    {
        return ErrorResponse.HandleFailure(Error.Conflict("Login.AlreadyLoggedIn", "Cannot login until you logout."));
    }

    var usernameExist = await userService.ExistByUsernameAsync(user.Username);

    if (!usernameExist)
    {
        return ErrorResponse.HandleFailure(Error.NotFount("Username", "The username does not exist"));
    }

    var isPasswordOk = await userService.CheckUserPasswordAsync(user);

    if (!isPasswordOk)
    {
        return ErrorResponse.HandleFailure(Error.Validation("Password", "The password is not correct"));
    }

    var result = await userService.LoginAsync(user);

    return Results.Ok(result);
});


app.MapPost("/auth/register", async ([FromBody] AuthRegisterDto user, [FromServices] IUserService userService, HttpContext http) =>
{
    var validationResult = ValidationHelper.Validate(user);
    if (validationResult is not null)
        return ErrorResponse.HandleFailure(Error.Validation(), validationResult);

    if (http.User?.Identity?.IsAuthenticated ?? false)
    {
        return ErrorResponse.HandleFailure(Error.Conflict("Register.AlreadyRegistered", "Cannot create an account until you logout"));
    }
    var usernameExist = await userService.ExistByUsernameAsync(user.Username);

    if (usernameExist)
    {
        return ErrorResponse.HandleFailure(Error.Conflict("Username", "Username already exists"));
    }
    await userService.AddUserAsync(user);
    return Results.Ok();
});


app.MapPost("/auth/tokens/refresh", async ([FromBody] AuthRefreshDto refresh, [FromServices] ITokenService tokenService, HttpContext http) =>
{
    var validationResult = ValidationHelper.Validate(refresh);
    if (validationResult is not null)
        return ErrorResponse.HandleFailure(Error.Validation(), validationResult);


    if (http.User?.Identity?.IsAuthenticated ?? false)
        return ErrorResponse.HandleFailure(Error.Conflict("Refresh.AlreadySingedIn", "Cannot refresh an account until you logout"));



    var userIdByToken = await tokenService.GetUserIdByValidTokenAsync(refresh.RefreshToken);

    if (!userIdByToken.HasValue)
    {
        return ErrorResponse.HandleFailure(Error.Conflict("Refresh.IncorrectRefreshToken", "Invalid refresh token, try to login again"));
    }

    var result = await tokenService.RefreshAsync(userIdByToken.Value);

    return Results.Ok(result);
});


app.MapGet("auth/health", () =>
{
    return Results.Ok(new
    {
        status = "ok",
        service = "user-service"
    });
});



app.Run();