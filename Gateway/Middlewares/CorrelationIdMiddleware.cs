
// namespace Gateway.Middlewares;

// public class CorrelationIdMiddleware
// {
//     public async Task InvokeAsync(HttpContext context)
//     {
//         var correlationId = context.Request.Headers["X-Correlation-Id"]
//                             .FirstOrDefault() ?? Guid.NewGuid().ToString();

//         context.Response.Headers["X-Correlation-Id"] = correlationId;
        

//         context.Request.Headers["X-Correlation-Id"] = correlationId;
        
//         await (context);
//     }
// }