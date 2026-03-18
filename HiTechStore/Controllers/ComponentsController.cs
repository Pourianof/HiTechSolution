using AutoMapper;

using HiTechStore.Controllers.ActionFilters;
using HiTechStore.Core;
using HiTechStore.Core.Exceptions;
using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Brand;
using HiTechStore.Data.DTOs.Component;
using HiTechStore.Helpers.Types;
using HiTechStore.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComponentsController : ControllerBase
{

    private IUnitOfWork _unitOfWork { get; }
    private IMapper _mapper { get; }

    public ComponentsController(IUnitOfWork unitOfWork, IMapper mapper) : base()
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ComponentTypeDto>>> GetAllComponents()
    {
        var components = await _unitOfWork.ComponentRepository.GetAllProjectedAsync();

        return Ok(components);
    }

    [HttpPost]
    [Authorize(Roles = $"{IdentityRoles.Manager}, {IdentityRoles.Admin}")]
    public async Task<ActionResult<ComponentTypeDto>> CreateComponent(ComponentCreationDto componentCreationDto)
    {
        var componentModel = _mapper.Map<ComponentType>(componentCreationDto);
        await _unitOfWork.ComponentRepository.AddAsync(componentModel);
        await _unitOfWork.Complete();

        return CreatedAtAction(nameof(GetComponent), new { id = componentModel.ComponentTypeId }, _mapper.Map<ComponentTypeDto>(componentModel));
    }

    [HttpDelete("{id}")]
    [TypeFilter<ResourceExistenceActionFilterAttribute<ComponentType>>]
    [Authorize(Roles = $"{IdentityRoles.Manager}, {IdentityRoles.Admin}")]
    public async Task<ActionResult> DeleteComponent(int id)
    {
        var model = HttpContext.Items["resource"] as ComponentType;

        await _unitOfWork.ComponentRepository.Delete(model!);
        await _unitOfWork.Complete();

        return NoContent();
    }

    [HttpGet("{id}")]
    [TypeFilter<ResourceExistenceActionFilterAttribute<ComponentType>>]
    public async Task<ActionResult<ComponentTypeDto>> GetComponent(int id)
    {
        var component = await _unitOfWork.ComponentRepository.GetByIdProjectedAsync(id);

        return component!;
    }

    [HttpPost("{id}/models")]
    [TypeFilter<ResourceExistenceActionFilterAttribute<ComponentType>>]
    [Authorize(Roles = $"{IdentityRoles.Manager}, {IdentityRoles.Admin}")]
    public async Task<ActionResult<ComponentModel>> CreateComponentModel(int id, ComponentModelCreationDto componentModelCreationDto)
    {
        var component = await _unitOfWork.ComponentRepository.GetModelByIdAsync(id);

        var model = _mapper.Map<ComponentModel>(componentModelCreationDto);
        BrandModelDto? brandModel = null;
        if (componentModelCreationDto.BrandModelId is not null)
        {
            brandModel = await _unitOfWork.BrandModelRepository.GetByIdProjectedAsync(componentModelCreationDto.BrandModelId.Value);
            if (brandModel is null)
            {
                return BadRequest(
                    new ProblemDetails()
                    {
                        Title = "not exist data reference",
                        Detail = $"There is no brand-model with id {componentModelCreationDto.BrandModelId}",
                        Status = StatusCodes.Status400BadRequest

                    }
                );
            }
        }

        if (component?.Properties == null || !component.Properties.Any())
        {
            model.Properties = new List<ComponentPropertyValue>();
        }
        else
        {
            for (int index = 0; index < (model.Properties ?? []).Count(); index++)
            {
                var prop = model.Properties!.ElementAt(index);
                var errorPropPath = $"Properties.{index}.PropertyId";

                var actualProperty = component.Properties.Where((p) => p.PropertyId == prop.PropertyId).FirstOrDefault();

                if (actualProperty is null)
                {
                    ModelState.AddModelError(errorPropPath, $"No property with id {prop.Property!.PropertyId} registered for component with id {id}");
                    var problem = new ValidationProblemDetails(ModelState)
                    {
                        Title = "Bad input",
                        Detail = "Property not found",
                        Status = StatusCodes.Status400BadRequest
                    };
                    return BadRequest(problem);
                }

                var propType = actualProperty.PropertyType;

                try
                {
                    prop.Value!.PopulateValue(propType);
                }
                catch (PropertyValueTypeDismatchException ex)
                {
                    ModelState.AddModelError(
                       errorPropPath,
                       ex.Message
                    );
                    var problem = new ValidationProblemDetails(ModelState);
                    return BadRequest(problem);
                }
            }
        }

        component!.ComponentModels!.Add(model);
        await _unitOfWork.Complete();

        var componentModelDto = _mapper.Map<ComponentModelDto>(model);
        componentModelDto.BrandModel = brandModel;
        return Ok(componentModelDto);
    }

    [HttpGet("{id}/models")]
    [TypeFilter<ResourceExistenceActionFilterAttribute<ComponentType>>]
    public async Task<ActionResult<IEnumerable<ComponentModelDto>>> GetComponentModels(int id)
    {
        var models = await _unitOfWork.ComponentRepository.GetComponentsModels(id);

        return Ok(models);
    }
}