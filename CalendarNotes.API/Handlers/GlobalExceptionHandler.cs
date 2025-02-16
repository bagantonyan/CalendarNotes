using CalendarNotes.API.Models.ApiModels;
using CalendarNotes.Shared.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace CalendarNotes.API.Handlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError($"An error occurred while processing the request: {exception.Message}");

            int statusCode;

            switch (exception)
            {
                case NotFoundException:
                    statusCode = (int)HttpStatusCode.NotFound;
                    break;

                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            httpContext.Response.StatusCode = statusCode;

            await httpContext
                .Response
                .WriteAsJsonAsync(ApiResponse<object>.Fail(exception.Message, statusCode), cancellationToken);

            return true;
        }
    }
}