using CollegeUni.Api.Filters;
using CollegeUni.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollegeUni.Api.Filters
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger _logger;
        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
        {
            _logger = logger;
        }
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is CustomException)
            {
                var customEx = context.Exception as CustomException;
                _logger.LogError(3, parseException(customEx));
                // Demonstrates an unhandled exception as an http response.
                if(!context.ExceptionHandled && customEx.StatusCode != 500)
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

        private string parseException(CustomException ex)
        {
            return string.Format("Message: {0}\nSource: {1}\nStack Track: {2}\nStatus Code: {3}\nModel State: {4}",
                ex.Message, ex.Source, ex.StackTrace, ex.StatusCode, ex.ModelState);
        }
    }
}
