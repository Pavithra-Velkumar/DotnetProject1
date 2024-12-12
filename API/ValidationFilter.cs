using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where( e => e.Value?.Errors.Count > 0)
                .Select( e => new
                {
                    Name = e.Key,
                    Message = e.Value?.Errors.First().ErrorMessage
                }).ToList();

            context.Result = new BadRequestObjectResult(errors);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}