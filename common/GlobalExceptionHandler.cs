
using CharityHubOnionArchitecture.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
namespace CharityHubOnionArchitecture.common
{
    public class GlobalExceptionHandler : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var response = new Dictionary<string, string>();
            int statusCode;

            // Handle specific custom exceptions
            switch (context.Exception)
            {
                case AppException.RequirementException:
                    statusCode = StatusCodes.Status409Conflict;
                    break;

                case AppException.BadRequestException:
                    statusCode = StatusCodes.Status400BadRequest;
                    break;

                case AppException.UnAuthorized:
                    statusCode = StatusCodes.Status401Unauthorized;
                    break;

                case AppException.NotFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    break;

                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    break;
            }

            // Set error details in response
            response["description"] = context.Exception.Message;
            context.HttpContext.Response.StatusCode = statusCode;
            context.Result = new JsonResult(response);

            // Log the exception (optional, replace with actual logging framework)
            Console.WriteLine($"Error: {context.Exception.Message}");
        }
    }

}
