using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using User_Service.Common.Utilities.Implementations;
using User_Service.Common.Utilities.Interfaces;
using User_Service.Data;
using User_Service.DTOs.AuthDTOs;
using User_Service.Services.Implementations;
using User_Service.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseAzureSql(builder.Configuration.GetConnectionString("Default"));
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("auth/login", async ([FromBody] AuthLoginDto user, [FromServices] IUserService userService) =>
{
    var validationResult = ValidationHelper.HandleValidation(user);
    if (validationResult is not null)
        return validationResult;

    var usernameExist = await userService.ExistByUsernameAsync(user.Username);

    if (!usernameExist)
    {
        return Results.BadRequest("The username is not exist");
    }

    var isPasswordOk = await userService.CheckUserPasswordAsync(user);

    if (!isPasswordOk)
    {
        return Results.BadRequest("The password is not correct");
    }

    var result = await userService.LoginAsync(user);

    return Results.Ok(result);
});


app.MapPost("auth/Register", async ([FromBody] AuthRegisterDto user, [FromServices] IUserService userService, HttpContext http) =>
{
    var validationResult = ValidationHelper.HandleValidation(user);
    if (validationResult is not null)
        return validationResult;

    if (http.User?.Identity?.IsAuthenticated ?? false)
    {
        return Results.BadRequest("cannot create an account until you logout");
    }
    var usernameExist = await userService.ExistByUsernameAsync(user.Username);

    if (usernameExist)
    {
        return Results.BadRequest("The username is already exist");
    }
    await userService.AddUserAsync(user);

    return Results.Ok(user);
});


app.MapPost("auth/refresh", async ([FromQuery] string refreshToken, [FromServices] TokenService tokenService, HttpContext http) =>
{
    if (http.User?.Identity?.IsAuthenticated ?? false)
        return Results.BadRequest("cannot refresh an account until you logout");

    if (refreshToken is null)
        return Results.BadRequest("Invalid refresh token");

    var userIdByToken = await tokenService.GetUserIdByValidTokenAsync(refreshToken);

    if (!userIdByToken.HasValue)
    {
        return Results.BadRequest("Invalid refresh token, try to login again");
    }

    var result = await tokenService.RefreshAsync(userIdByToken.Value);

    return Results.Ok(result);
});

app.Run();