using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;

namespace BackEndProducts.Api.Model
{
    public class CustomProblemDetailsFactory : ProblemDetailsFactory
    {
        /// <summary>
        /// TO controlate specific errors calling input payload api with no standard parameters o incompatible types, and avoid return implementation details to front
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="statusCode"></param>
        /// <param name="title"></param>
        /// <param name="type"></param>
        /// <param name="detail"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public override ProblemDetails CreateProblemDetails(HttpContext httpContext, int? statusCode = null, string title = null, string type = null, string detail = null, string instance = null)
        {
            ProblemDetails problemDetails = new ProblemDetails
            {
                Title = "Ups! Unhandled error",
                Status = 400
            };

            return problemDetails;
        }

        /// <summary>
        /// TO controlate specific errors calling input payload api with no standard parameters o incompatible types, and avoid return implementation details to front
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="modelStateDictionary"></param>
        /// <param name="statusCode"></param>
        /// <param name="title"></param>
        /// <param name="type"></param>
        /// <param name="detail"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public override ValidationProblemDetails CreateValidationProblemDetails(HttpContext httpContext, ModelStateDictionary modelStateDictionary, int? statusCode = null, string title = null, string type = null, string detail = null, string instance = null)
        {
            ValidationProblemDetails customError = new ValidationProblemDetails
            {
                Title = "Unhandled error!",
                Status = 400
            };

            return customError;
        }
    }
}
