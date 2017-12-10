using CollegeUni.Api.Filters;
using CollegeUni.Services.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace CollegeUni.Filters
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger _logger;
        public ApiExceptionFilterAttribute(ILogger<ApiExceptionFilterAttribute> logger)
        {
            _logger = logger;
        }
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is ApiResponseException)
            {
                var customEx = context.Exception as ApiResponseException;
                _logger.LogError(3, ParseException(customEx));
                _logger.LogError(customEx, "Logged Custom Exception");

                // Demonstrates an unhandled exception as an http response.
                if (!context.ExceptionHandled && customEx.StatusCode != 500)
                {
                    var serviceResult = new ServiceResult
                    {
                        Message = customEx.Message,
                        ModelState = customEx.ModelState
                    };
                    context.Result = new ObjectResult(serviceResult) { StatusCode = customEx.StatusCode };
                }
            }
        }

        private string ParseException(ApiResponseException ex)
        {
            return string.Format("\nMessage: {0}\nSource: {1}\nStack Trace: {2}\nStatus Code: {3}\nModel State: {4}",
                ex.Message, ex.Source, ex.StackTrace, ex.StatusCode, ex.ModelState);
        }
    }
}