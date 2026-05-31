using HiTechStore.Core;
using HiTechStore.Infrastructure.Data.DTOs;
using HiTechStore.Infrastructure.Data.DTOs.DiscountEntity;

using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Presentation.Controllers;

[Route("api/discounts/entities")]
[ApiController]
public class DiscountEntitiesController(IUnitOfWork unitOfWork) : ControllerBase
{
    private IUnitOfWork _unitOfWork = unitOfWork;

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<DiscountEntityDto>>> GetDiscountEntities()
    {
        return Ok(await _unitOfWork.DiscountEntityRepository.GetPagedProjectedAsync(0));
    }
}
