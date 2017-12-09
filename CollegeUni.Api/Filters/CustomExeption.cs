using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollegeUni.Api.Filters
{
    public class CustomException : Exception
    {
        public CustomException() : base() { }
        public CustomException(string message, ModelStateDictionary modelState = null, int statusCode = 500) : base(message)
        {
            ModelState = modelState;
            StatusCode = statusCode;
        }
        public virtual ModelStateDictionary ModelState { get; protected set; }
        public virtual int StatusCode { get; protected set; } = 500;

        public override string HelpLink { get => base.HelpLink; set => base.HelpLink = value; }

        public override string Message => base.Message;

        public override string Source { get => base.Source; set => base.Source = value; }

        public override string StackTrace => base.StackTrace;
    }
}
