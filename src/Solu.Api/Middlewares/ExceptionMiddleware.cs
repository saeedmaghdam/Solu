using System;
using System.Threading.Tasks;
using Solu.Api.Exceptions;
using Solu.Framework.Exceptions;
using Solu.Api.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Solu.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (ApiException ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
            catch (ValidationException ex)
            {
                await HandleValidationAsync(httpContext, ex);
            }
            catch (Exception exception)
            {
                await HandleUnknownExceptionAsync(httpContext, exception);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, ApiException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception.StatusCode;
            return context.Response.WriteAsync(JsonConvert.SerializeObject(ApiResultViewModel<object>.FromError(
                new ErrorViewModel { ErrorCode = exception.ErrorCode, ErrorDescription = exception.ErrorDescription })));
        }
        private Task HandleValidationAsync(HttpContext context, ValidationException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return context.Response.WriteAsync(JsonConvert.SerializeObject(ApiResultViewModel<object>.FromError(
                new ErrorViewModel { ErrorCode = exception.ErrorCode, ErrorDescription = exception.ErrorDescription })));
        }
        private Task HandleUnknownExceptionAsync(HttpContext httpContext, Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception occurred in request {TraceIdentifier}", httpContext.TraceIdentifier);

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = 500;
            return httpContext.Response.WriteAsync(JsonConvert.SerializeObject(ApiResultViewModel<object>.FromError(new InternalErrorViewModel(exception.Message, httpContext.TraceIdentifier, exception.ToString()))));
        }
    }
}
