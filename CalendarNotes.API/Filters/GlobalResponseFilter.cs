using CalendarNotes.API.Models.ApiModels;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using CalendarNotes.API.Extensions;
using Newtonsoft.Json;

namespace CalendarNotes.API.Filters
{
    public class GlobalResponseFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult objectResult)
            {
                if (context.HttpContext.Response.StatusCode.IsSuccess())
                {
                    var responseModel = ApiResponse<object>
                        .Success(objectResult.Value, context.HttpContext.Response.StatusCode);

                    context.Result = new ObjectResult(responseModel);
                }
                else
                {
                    var responseModel = ApiResponse<object>
                        .Fail(JsonConvert.SerializeObject(context.HttpContext.Response.Body), context.HttpContext.Response.StatusCode);

                    context.Result = new ObjectResult(responseModel);
                }
                
            }
            else if (context.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode.IsSuccess())
                {
                    var responseModel = ApiResponse<object>
                        .Success(null, context.HttpContext.Response.StatusCode);

                    context.Result = new ObjectResult(responseModel);
                }
                else
                {
                    var responseModel = ApiResponse<object>
                        .Fail(JsonConvert.SerializeObject(context.HttpContext.Response.Body), context.HttpContext.Response.StatusCode);

                    context.Result = new ObjectResult(responseModel);
                }
            }
        }
    }
}