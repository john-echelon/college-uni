using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollegeUni.Models
{
    public class ServiceResult
    {
        public string Message { get; set; }
        /// <summary>
        /// ModelState hidden from Json serialiation. Override ParseModelState to alter serialization of the ModelState.
        /// </summary>
        [JsonIgnore]
        public ModelStateDictionary ModelState { protected get; set; }
        public object Errors
        {
            get
            {
                return ParseModelState(ModelState);
            }
        }
        protected virtual object ParseModelState (ModelStateDictionary modelState)
        {
            var errorList = ModelState?.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );
            return errorList;
        }
    }
    public class ServiceResult<T>: ServiceResult
    {
        public T Data { get; set; }
    }
}
