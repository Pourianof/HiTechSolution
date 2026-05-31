using HiTechStore.Core;
using HiTechStore.Core.Models;

using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ColorsController(IUnitOfWork unitOfWork) : ControllerBase
{
    private IUnitOfWork _unitOfWork = unitOfWork;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Color>>> GetAllColors()
    {
        return Ok(await _unitOfWork.ColorRepository.GetAllAsync());
    }
}