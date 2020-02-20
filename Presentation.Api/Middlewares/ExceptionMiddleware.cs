using Core.Exceptions;
using Data.Models.Mapping.Wrappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Presentation.Api.Middlewares
{
    /// <summary>
    /// 
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="localizer"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext httpContext, IStringLocalizer<Exceptions> localizer)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, localizer, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, IStringLocalizer<Exceptions> localizer, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = new Response()
            {
                Status = ApiResponseStatus.Error,
                StackTrace = exception.StackTrace,
                Message = exception.Message,
            };
            var baseException = exception as BaseException;
            if (baseException != null)
            {
                context.Response.StatusCode = (int)baseException.StatusCode;
                response.ErrorCode = baseException.ErrorCode;
                response.Message = localizer[baseException.Key].Value;
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return context.Response.WriteAsync(response.ToString());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class ExceptionMiddlewareExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>

        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
        {

            return builder.UseMiddleware<ExceptionMiddleware>();

        }
    }

}
