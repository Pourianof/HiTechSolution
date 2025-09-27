using AutoMapper;

using HiTechStore.Controllers.ActionFilters;
using HiTechStore.Core;
using HiTechStore.Data.DTOs.Component;
using HiTechStore.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Controllers;

[ApiController]
[Route("/[controller]")]
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
        var components = await _unitOfWork.ComponentRepository.GetAllAsync() ?? [];

        return Ok(components);
    }

    [HttpGet("{id}")]
    [TypeFilter<ResourceExistenceActionFilterAttribute<ComponentType>>]
    public async Task<ActionResult<ComponentTypeDto>> GetComponent(int id)
    {
        var component = await _unitOfWork.ComponentRepository.GetByIdAsync(id);

        return component!;
    }

    [HttpPost("{id}/models")]
    [TypeFilter<ResourceExistenceActionFilterAttribute<ComponentType>>]
    [Authorize(Roles = $"{IdentityRoles.Manager}, {IdentityRoles.Admin}")]
    public async Task<ActionResult<ComponentModel>> CreateComponentModel(int id, ComponentModelCreationDto componentModelDto)
    {
        var component = await _unitOfWork.ComponentRepository.GetModelByIdAsync(id);

        var model = _mapper.Map<ComponentModel>(componentModelDto);
        component!.ComponentModels!.Append(model);
        await _unitOfWork.Complete();

        return Ok(model);
    }

    [HttpGet("{id}/models")]
    [TypeFilter<ResourceExistenceActionFilterAttribute<ComponentType>>]
    public async Task<ActionResult<IEnumerable<ComponentModelDto>>> GetComponentModels(int id)
    {
        var models = await _unitOfWork.ComponentRepository.GetComponentsModels(id);

        return Ok(models);
    }
}