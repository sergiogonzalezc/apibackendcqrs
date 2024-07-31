using BackEndProducts.Common;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Diagnostics;

namespace BackEndProducts.Api.Model
{
   
    /// <summary>
    /// Logea cada ejecución de caulquier request
    /// </summary>
    public class EndpointExecutionFilter : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(
            EndpointFilterInvocationContext context,
            EndpointFilterDelegate next
        )
        {
            if (context?.HttpContext?.Request is not null)
            {                
                var httpMethod = context.HttpContext.Request.Method;
                var host = context.HttpContext.Request.Host.Value;
                var path = context.HttpContext.Request.Path.Value;                

                var requestLog = $"REQUEST httpMethod: {httpMethod}, Host: {host}, Path: {path}";
                ServiceLog.Write(Common.Enum.LogType.WebSite, TraceLevel.Info, nameof(InvokeAsync), requestLog);
            }

            return await next(context);
        }
    }

}