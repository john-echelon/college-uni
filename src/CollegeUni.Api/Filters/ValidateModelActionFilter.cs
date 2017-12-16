using CollegeUni.Services.Models;
using CollegeUni.Utilities.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CollegeUni.Filters
{
    public class ValidateModelActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Not needed but required to interface IActionFilter.
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var serviceResult = new ServiceResult
                {
                    ModelState = context.ModelState.ToStringDictionary()
                };
                context.Result = new BadRequestObjectResult(serviceResult);
            }
        }
    }
}