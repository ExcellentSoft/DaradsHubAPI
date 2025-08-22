using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Domain.Entities;
using System.Net;

namespace DaradsWebMobAPIs.WebAPI.Middleware;

public class ExceptionMiddleware(RequestDelegate _next)
{
    public async Task InvokeAsync(HttpContext context, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment env)

    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
            await HandleExceptionAsync(context, ex, env);
            return;
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception, IWebHostEnvironment env)
    {
        var error = new ErrorVM();
        HttpStatusCode status;

        if (exception is AppException)
        {
            var ex = (exception as AppException)!;
            error.Message = ex.Message;
            error.Detail = ex.ToString();
            error.ErrorItems = ex.ErrorItems;
            status = HttpStatusCode.BadRequest;
        }
        else
        {
            if (!env.IsDevelopment())
            {
                error.Message = "An error was encountered.";
                error.Detail = "";
            }
            else
            {
                error.Message = exception.Message;
                error.Detail = exception.ToString();
            }
            status = HttpStatusCode.InternalServerError;
        }
        context.Response.StatusCode = (int)status;
        await context.Response.WriteAsJsonAsync(new BaseResponse<List<string>>(false, error.Message, error.ErrorItems!.ToList()));
    }
}