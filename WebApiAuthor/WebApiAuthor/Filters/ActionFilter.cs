using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiAuthor.Filters
{
    public class ActionFilter : IActionFilter
    {
        private readonly ILogger<ActionFilter> _logger;

        public ActionFilter(ILogger<ActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("After execute the Action");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("Before execute the Action");
        }
    }
}

