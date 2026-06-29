

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));


builder.Services.AddCors(options =>
{
    options.AddPolicy("Default", builder =>
    {
        builder.AllowAnyOrigin();
        builder.AllowAnyHeader();
        builder.AllowAnyMethod();
    });

});

var app = builder.Build();

app.UseCors("Default");

app.Use(async (context, next) =>
{
    const string headerName = "X-Correlation-Id";

    var correlationId =
        context.Request.Headers.TryGetValue(headerName, out var existing)
            ? existing.ToString()
            : Guid.NewGuid().ToString();

    context.Request.Headers[headerName] = correlationId;

    context.Response.OnStarting(() =>
    {
        context.Response.Headers[headerName] = correlationId;
        return Task.CompletedTask;
    });

    await next();
});

app.MapReverseProxy();

app.Run();
