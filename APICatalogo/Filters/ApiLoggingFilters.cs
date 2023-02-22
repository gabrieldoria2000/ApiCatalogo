using Microsoft.AspNetCore.Mvc.Filters;

namespace APICatalogo.Filters
{
    public class ApiLoggingFilters : IActionFilter
    {
        private readonly ILogger<ApiLoggingFilters> _logger;

        public ApiLoggingFilters(ILogger<ApiLoggingFilters> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //ANTES DA ACTION
            _logger.LogInformation("### Executando - > OnActionExecuting");
            _logger.LogInformation("###");
            _logger.LogInformation($"{DateTime.Now.ToLongTimeString()}");
            _logger.LogInformation($"ModelState : {context.ModelState.IsValid}");
            _logger.LogInformation("###");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            //DEPOIS DA ACTION
            _logger.LogInformation("### Executando - > OnActionExecuted");
            _logger.LogInformation("###");
            _logger.LogInformation($"{DateTime.Now.ToLongTimeString()}");
            _logger.LogInformation("###");
        }

        
    }
}
