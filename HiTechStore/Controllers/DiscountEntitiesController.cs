using HiTechStore.Core;
using HiTechStore.Models;

using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Controllers;

[Route("api/discount/entities")]
[ApiController]
public class DiscountEntitiesController(IUnitOfWork unitOfWork) : ControllerBase
{
    private IUnitOfWork _unitOfWork = unitOfWork;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DiscountEntity>>> GetDiscountEntities()
    {
        return Ok(await _unitOfWork.DiscountEntityRepository.GetAllProjectedAsync());
    }
}
