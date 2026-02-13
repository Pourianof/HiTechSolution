using HiTechStore.Core;
using HiTechStore.Data.DTOs.DiscountEntity;

using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Controllers;

[Route("api/discounts/entities")]
[ApiController]
public class DiscountEntitiesController(IUnitOfWork unitOfWork) : ControllerBase
{
    private IUnitOfWork _unitOfWork = unitOfWork;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DiscountEntityDto>>> GetDiscountEntities()
    {
        return Ok(await _unitOfWork.DiscountEntityRepository.GetAllProjectedAsync());
    }
}
