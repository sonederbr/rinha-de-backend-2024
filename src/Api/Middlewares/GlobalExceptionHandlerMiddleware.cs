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
        catch (Exception ex)
        {
            Log.Error(ex, $"Global Exception");

            var environment = context.RequestServices.GetService<IWebHostEnvironment>();

            var problemDetails = new ProblemDetails
            {
                Type = ex.GetType().ToString(),
                Title = ex.Message,
                Status = StatusCodes.Status500InternalServerError
            };

            // if(environment?.IsDevelopment() == true)
            // {
                problemDetails.Type = ex.GetType().ToString();
                problemDetails.Title = ex.Message;
                problemDetails.Detail = ex.StackTrace;
            // }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions()
            {
                MaxDepth = 3,
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            }));
        }
    }
}