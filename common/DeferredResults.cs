
using System;
using System.Threading.Tasks;
using CharityHubOnionArchitecture.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CharityHubOnionArchitecture.common
{
    public static class DeferredResults
    {
        /// <summary>
        /// Converts a Task into an ActionResult, properly handling success and exceptions.
        /// </summary>
        /// <typeparam name="T">The type of the result returned by the Task.</typeparam>
        /// <param name="task">The asynchronous task to convert.</param>
        /// <returns>An asynchronous ActionResult that resolves when the Task completes.</returns>
        public static async Task<IActionResult> From<T>(Task<T> task)
        {
            try
            {
                var result = await task.ConfigureAwait(false);
                return new OkObjectResult(result); // Returns 200 OK with the result
            }
            catch (Exception ex) when (ex is AggregateException || ex is TaskCanceledException || ex is OperationCanceledException)
            {
                return HandleException(ex.InnerException ?? ex);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Handles exceptions and converts them into an appropriate ActionResult.
        /// </summary>
        /// <param name="exception">The exception to handle.</param>
        /// <returns>An ActionResult representing the error.</returns>
        private static IActionResult HandleException(Exception exception)
        {
            // Map exceptions to appropriate HTTP status codes
            if (exception is AppException.BadRequestException)
            {
                return new BadRequestObjectResult(new { description = exception.Message });
            }

            if (exception is AppException.UnAuthorized)
            {
                return new UnauthorizedObjectResult(new { description = exception.Message });
            }

            if (exception is AppException.NotFoundException)
            {
                return new NotFoundObjectResult(new { description = exception.Message });
            }

            // Default to 500 Internal Server Error for unhandled exceptions
            return new ObjectResult(new { description = exception.Message })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }

}
