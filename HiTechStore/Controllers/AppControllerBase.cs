using System.ComponentModel.DataAnnotations;

using HiTechStore.Core.Helpers.Result;
using HiTechStore.Presentation.Helpers.Result;

using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Controllers;

[ApiController]
public class AppControllerBase : ControllerBase
{
    protected BadRequestObjectResult ValidationResult(IEnumerable<ValidationResultError> errors)
    {
        var problem = ResultValidationMapper.ToValidationProblemDetails(errors, "Password change failed");
        return BadRequest(problem);
    }
}