using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using static DepsWebApp.Mappings.ErrorMapping;

namespace DepsWebApp.Filters
{
    public class CustomExceptionFilter : ExceptionFilterAttribute, IExceptionFilter
    {
        public override void OnException(ExceptionContext context)
        {
            var compared = context.Exception.GetType();

            var exceptionName = compared.Name.ToString();

            var errorDetails = MapError(exceptionName);

            context.Result = new JsonResult(errorDetails);
        }
    }
}
