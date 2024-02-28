namespace Api.Middlewares;

public class GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> logger) : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (BadHttpRequestException bex)
        {
            await ProduceException(bex, StatusCodes.Status400BadRequest);
        }
        catch (Exception ex)
        {
            await ProduceException(ex, StatusCodes.Status500InternalServerError);
        }

        return;

        async Task ProduceException(Exception ex, int statusCode)
        {
            var problemDetails = new ProblemDetails
            {
                Type = ex.GetType().ToString(),
                Title = ex.Message,
                Status = statusCode
            };

            if (context.RequestServices.GetService<IWebHostEnvironment>()?.IsDevelopment() == true)
            {
                problemDetails.Type = ex.GetType().ToString();
                problemDetails.Title = ex.Message;
                problemDetails.Detail = ex.StackTrace;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions()
            {
                MaxDepth = 3,
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            }));
        }
    }
}