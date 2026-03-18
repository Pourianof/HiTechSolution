using AutoMapper;

using HiTechStore.Controllers.ActionFilters;
using HiTechStore.Core;
using HiTechStore.Data.DTOs.Brand;
using HiTechStore.Helpers.IO;
using HiTechStore.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{IdentityRoles.Admin},{IdentityRoles.Manager}")]
public class BrandsController : ControllerBase
{
    private IUnitOfWork _unitOfWork { get; }
    private IMapper _mapper { get; }

    public BrandsController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    private string? ProvideBrandImage(string? brandName)
    {
        var imagePath = $"images/brands/{brandName}.png";
        return PublicAssetsHelper.IsExist(imagePath) ? imagePath : null;

    }

    [HttpGet]
    [AllowAnonymous]
    public async IAsyncEnumerable<BrandDto> GetBrands()
    {
        var brands = await _unitOfWork.BrandRepository.GetPagedProjectedAsync();
        foreach (var brand in brands.Items)
        {
            brand.Image = ProvideBrandImage(brand.Name);
            yield return brand;
        }
    }

    [HttpGet("{id}")]
    [TypeFilter<ResourceExistenceActionFilterAttribute<Brand>>]
    public async Task<BrandDto> GetBrand(int id)
    {
        var brand = await _unitOfWork.BrandRepository.GetByIdProjectedAsync(id)!;
        brand!.Image = ProvideBrandImage(brand.Name);

        return brand;
    }


    [HttpPost]
    public async Task<ActionResult<BrandDto>> CreateBrand(BrandCreationDto brandDto)
    {
        var brand = _mapper.Map<Brand>(brandDto);

        string? imagePath = null;
        if (brandDto.Image is not null)
        {
            imagePath = $"images/brands/{brand.Name}.png";
            await PublicAssetsHelper.WriteIFormFile(brandDto.Image, imagePath);
        }

        await _unitOfWork.BrandRepository.AddAsync(brand);
        await _unitOfWork.Complete();

        var createdBrandDto = _mapper.Map<BrandDto>(brand);
        createdBrandDto.Image = imagePath;

        return CreatedAtAction(nameof(GetBrand), new { id = createdBrandDto.BrandId }, createdBrandDto);
    }

    [HttpDelete("{brandId}")]
    [TypeFilter<ResourceExistenceActionFilterAttribute<Brand>>]
    public async Task<ActionResult<BrandDto>> DeleteBrand(int brandId)
    {
        await _unitOfWork.BrandRepository.Delete(brandId);
        await _unitOfWork.Complete();

        return NoContent();
    }

    [AllowAnonymous]
    [HttpGet("{brandId}/models")]
    [TypeFilter<ResourceExistenceActionFilterAttribute<Brand>>]
    public async Task<ActionResult<IEnumerable<BrandModelDto>>> GetBrandsModels(int brandId)
    {
        var brandModels = await _unitOfWork.BrandModelRepository.GetModelsOfSingleBrand(brandId) ?? [];

        return Ok(brandModels);
    }

    [HttpPost("{brandId}/models")]
    [TypeFilter<ResourceExistenceActionFilterAttribute<Brand>>]
    public async Task<ActionResult<BrandModelDto>> RegisterNewModel(int brandId, BaseBrandModelCreationDto modelCreationDto)
    {
        var brand = await _unitOfWork.BrandRepository.GetModelByIdAsync(brandId);

        var brandModel = _mapper.Map<BrandModel>(modelCreationDto);
        brand!.Models!.Add(brandModel);

        await _unitOfWork.Complete();

        return Ok(_mapper.Map<BrandModelDto>(brandModel));
    }

}

