using Microsoft.AspNetCore.Diagnostics;

namespace TRIAS.NET.WebAPI.Helper;

public class TriasExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not TriasException triasException)
        {
            return false;
        }

        httpContext.Response.StatusCode = triasException.StatusCode;
        await httpContext.Response.WriteAsync(exception.Message, cancellationToken);

        return true;
    }
}
