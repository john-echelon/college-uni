using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollegeUni.Services.Models
{
    public class ServiceResult
    {
        public string Message { get; set; }
        /// <summary>
        /// ModelState should be hidden from serialiation. Override ParseModelState to alter serialization of the ModelState.
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, string[]> ModelState { protected get; set; }
        public bool HasErrors => ModelState.Any();
        public object Errors
        {
            get
            {
                return ParseModelState(ModelState);
            }
        }
        protected virtual object ParseModelState(Dictionary<string, string[]> modelState)
        {
            return modelState;
        }
    }
    public class ServiceResult<T> : ServiceResult
    {
        public T Data { get; set; }
    }
}
