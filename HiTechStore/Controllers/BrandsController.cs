using HiTechStore.Core;
using HiTechStore.Data.DTOs.Brand;
using HiTechStore.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Controllers;

[ApiController]
[Route("/brands")]
[Authorize(Roles = $"{IdentityRoles.Admin},{IdentityRoles.Manager}")]
public class BrandsController : ControllerBase
{
    private IUnitOfWork _unitOfWork { get; set; }
    public BrandsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // [HttpPost]
    // public IResult CreateBrand(CreateBrandDto brandDto)
    // {

    // }

}