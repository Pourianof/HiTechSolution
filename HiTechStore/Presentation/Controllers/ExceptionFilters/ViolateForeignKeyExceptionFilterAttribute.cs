using HiTechStore.Helpers.Types;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Presentation.Controllers.ExceptionFilters
{
    public class ViolateForeignKeyExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var dbUpdateException = context.Exception.GetBaseExceptionOfType<DbUpdateException>();
            if (dbUpdateException != null && dbUpdateException.ToString().Contains("violates foreign key constraint"))
            {
                var problem = new ProblemDetails
                {
                    Title = "Bad input data",
                    Detail = "One or more categories do not exist."
                };
                context.Result = new BadRequestObjectResult(problem);
                context.ExceptionHandled = true;
            }
        }
    }
}