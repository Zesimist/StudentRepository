using Microsoft.AspNetCore.Mvc.Filters;

namespace CrudExample.Filter.ActionFilters
{
    public class PersonListActionFilter: IActionFilter
    {
        public PersonListActionFilter() {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine("hello from executed");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine("hello from executing");
        }
    }

}
