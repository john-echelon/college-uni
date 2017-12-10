using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace CollegeUni.Utilities.Extensions
{
    public static class ModelStateExtensions
    {
        public static Dictionary<string, string[]> ToStringDictionary(this ModelStateDictionary modelState) => modelState?.ToDictionary(
              kvp => kvp.Key,
              kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray());
    }
}
