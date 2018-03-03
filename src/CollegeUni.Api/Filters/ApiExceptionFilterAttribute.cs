using CollegeUni.Api.Filters;
using CollegeUni.Services.Models;
using CollegeUni.Utilities.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using NLog;

namespace CollegeUni.Filters
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        public ApiExceptionFilterAttribute(ILogger<ApiExceptionFilterAttribute> logger)
        {
            _logger = logger;
        }
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is ApiResponseException)
            {
                var customEx = context.Exception as ApiResponseException;

                // Demonstrates an unhandled exception as an http response.
                if (!context.ExceptionHandled && customEx.StatusCode < 500)
                {
                    var serviceResult = new ServiceResult
                    {
                        Message = customEx.Message,
                        ModelState = customEx.ModelState
                    };
                    context.Result = new ObjectResult(serviceResult) { StatusCode = customEx.StatusCode };
                }
                var request = context?.HttpContext?.Request;
                if (request != null)
                {
                    MappedDiagnosticsContext.Set("RequestContext", new
                    {
                        request.Scheme,
                        Host = request.Host.Value,
                        Path = request.Path.Value,
                        request.QueryString
                    });
                }
            }
            _logger.LogError("Uncaught exception in api", context.Exception);
        }
    }
}