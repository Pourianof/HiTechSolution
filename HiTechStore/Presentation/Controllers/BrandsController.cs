using AutoMapper;

using HiTechStore.Presentation.Controllers.ActionFilters;
using HiTechStore.Core;
using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Infrastructure.Data.DTOs.Brand;
using HiTechStore.Core.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{IdentityRoles.Admin},{IdentityRoles.Manager}")]
public class BrandsController : ControllerBase
{
    private IUnitOfWork UnitOfWork { get; }
    private IMapper Mapper { get; }
    private IPublicAssetRegisterer AssetRegisterer { get; }

    public BrandsController(IUnitOfWork unitOfWork, IMapper mapper, IPublicAssetRegisterer assetRegisterer)
    {
        UnitOfWork = unitOfWork;
        Mapper = mapper;
        AssetRegisterer = assetRegisterer;
    }

    private string? ProvideBrandImage(string? brandName)
    {
        var imagePath = $"images/brands/{brandName}.png";
        return AssetRegisterer.IsExist(imagePath) ? imagePath : null;

    }

    [HttpGet]
    [AllowAnonymous]
    public async IAsyncEnumerable<BrandDto> GetBrands()
    {
        var brands = await UnitOfWork.BrandRepository.GetPagedProjectedAsync();
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
        var brand = await UnitOfWork.BrandRepository.GetByIdProjectedAsync(id)!;
        brand!.Image = ProvideBrandImage(brand.Name);

        return brand;
    }


    [HttpPost]
    public async Task<ActionResult<BrandDto>> CreateBrand(BrandCreationDto brandDto)
    {
        var brand = Mapper.Map<Brand>(brandDto);

        string? imagePath = null;
        if (brandDto.Image is not null)
        {
            imagePath = $"images/brands/{brand.Name}.png";
            using var brandImageStream = brandDto.Image.OpenReadStream();
            await AssetRegisterer.SaveFileAsync(new AppFile
            {
                File = brandImageStream,
                FileName = brandDto.Image.FileName
            }, imagePath);
        }

        await UnitOfWork.BrandRepository.AddAsync(brand);
        await UnitOfWork.Complete();

        var createdBrandDto = Mapper.Map<BrandDto>(brand);
        createdBrandDto.Image = imagePath;

        return CreatedAtAction(nameof(GetBrand), new { id = createdBrandDto.BrandId }, createdBrandDto);
    }

    [HttpDelete("{brandId}")]
    [TypeFilter<ResourceExistenceActionFilterAttribute<Brand>>]
    public async Task<ActionResult<BrandDto>> DeleteBrand(int brandId)
    {
        await UnitOfWork.BrandRepository.Delete(brandId);
        await UnitOfWork.Complete();

        return NoContent();
    }

    [AllowAnonymous]
    [HttpGet("{brandId}/models")]
    [TypeFilter<ResourceExistenceActionFilterAttribute<Brand>>]
    public async Task<ActionResult<IEnumerable<BrandModelDto>>> GetBrandsModels(int brandId)
    {
        var brandModels = await UnitOfWork.BrandModelRepository.GetModelsOfSingleBrand(brandId) ?? [];

        return Ok(brandModels);
    }

    [HttpPost("{brandId}/models")]
    [TypeFilter<ResourceExistenceActionFilterAttribute<Brand>>]
    public async Task<ActionResult<BrandModelDto>> RegisterNewModel(int brandId, BaseBrandModelCreationDto modelCreationDto)
    {
        var brand = await UnitOfWork.BrandRepository.GetModelByIdAsync(brandId);

        var brandModel = Mapper.Map<BrandModel>(modelCreationDto);
        brand!.Models!.Add(brandModel);

        await UnitOfWork.Complete();

        return Ok(Mapper.Map<BrandModelDto>(brandModel));
    }

}

