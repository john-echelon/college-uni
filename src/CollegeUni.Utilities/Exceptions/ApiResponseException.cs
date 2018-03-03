using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CollegeUni.Utilities.Extensions
{
    [Serializable]
    // Important: This attribute is NOT inherited from Exception, and MUST be specified 
    // otherwise serialization will fail with a SerializationException stating that
    // "Type X in Assembly Y is not marked as serializable."
    public class ApiResponseException : Exception
    {
        private readonly Dictionary<string, string[]> errors;
        private readonly int statusCode = 500;
        public ApiResponseException() : base() { }
        public ApiResponseException(string message) : base(message) { }
        public ApiResponseException(string message, Exception inner) : base(message, inner) { }

        public ApiResponseException(string message, Dictionary<string, string[]> errors, int statusCode = 500) : base(message)
        {
            this.errors = errors;
            this.statusCode = statusCode;
        }
        // Constructor should be protected for unsealed classes, private for sealed classes.
        // (The Serializer invokes this constructor through reflection, so it can be private)
        protected ApiResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            statusCode = info.GetInt32("StatusCode");
            errors = (Dictionary<string, string[]>)info.GetValue("ModelState", typeof(Dictionary<string, string[]>));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("StatusCode", StatusCode);
            info.AddValue("ModelState", ModelState);
            base.GetObjectData(info, context);
        }

        public int StatusCode
        {
            get
            {
                return this.statusCode;
            }
        }
        public Dictionary<string, string[]> ModelState
        {
            get
            {
                return errors;
            }
        }
    }
}