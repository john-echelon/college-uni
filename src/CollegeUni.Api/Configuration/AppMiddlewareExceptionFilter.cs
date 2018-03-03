using CollegeUni.Utilities.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Text;

namespace CollegeUni.Api.Configuration
{
    public static class AppMiddlewareExceptionFilter
    {
        public static Action<IApplicationBuilder> JsonHandler()
        {
            return errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exception = context.Features.Get<IExceptionHandlerFeature>();

                    if (exception != null && exception.Error is ApiResponseException)
                    {
                        var apiResponseEx = exception.Error as ApiResponseException;
                        if (apiResponseEx.StatusCode < 500)
                        {
                            context.Response.StatusCode = apiResponseEx.StatusCode;
                            // TODO: Massage the payload for dev environments vs prod 
                            var exceptionJson = Encoding.UTF8.GetBytes(
                                JsonConvert.SerializeObject(apiResponseEx,
                                new JsonSerializerSettings
                                {
                                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                                })
                            );
                            context.Response.ContentType = "application/json";
                            await context.Response.Body.WriteAsync(exceptionJson, 0, exceptionJson.Length);
                        }
                    }
                });
            };
        }
    }
}
