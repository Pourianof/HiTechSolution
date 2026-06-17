using System.Net;

using AutoMapper;

using HiTechStore.Presentation.Controllers.ActionFilters;
using HiTechStore.Core;
using HiTechStore.Infrastructure.Data.DTOs;
using HiTechStore.Infrastructure.Data.DTOs.Category;
using HiTechStore.Infrastructure.Data.DTOs.Component;
using HiTechStore.Infrastructure.Data.Storage;
using HiTechStore.DTOs.Category;
using HiTechStore.Helpers.Types;
using HiTechStore.Core.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Helpers.URLFilterQuery;
using HiTechStore.Infrastructure.Data.Queries;

namespace HiTechStore.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = $"{IdentityRoles.Admin},{IdentityRoles.Manager}")]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private ICategoryAssetHelper CategoryAssetHelper { get; }

        public CategoriesController(IUnitOfWork unitOfWork, IMapper mapper, ICategoryAssetHelper categoryAssetHelper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            CategoryAssetHelper = categoryAssetHelper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories([ToQuery] BaseQuery query)
        {
            var categories = await _unitOfWork.Categories.GetAllProjectedAsync(query);
            categories.Items = categories.Items.Select(
               (cat) =>
               {
                   var categoryDto = _mapper.Map<CategoryDTO>(cat);
                   categoryDto.Image = CategoryAssetHelper.GetCategoryImagePathIfExist(cat.CategoryId);
                   categoryDto.Icon = CategoryAssetHelper.GetCategoryIconPathIfExist(cat.CategoryId);
                   return categoryDto;
               }
           );
            return Ok(categories);
        }


        [HttpPost]
        public async Task<ActionResult<CategoryDTO>> CreateCategory([FromForm] CategoryCreationDto createCategoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = new Category
            {
                Name = createCategoryDto.Name,
                Description = createCategoryDto.Description,
            };

            if (createCategoryDto.Properties is not null)
            {
                category.Properties = _mapper.Map<List<Property>>(createCategoryDto.Properties);
            }


            if (createCategoryDto.Components is not null)
            {
                category.Components = _mapper.Map<List<CategoryComponent>>(createCategoryDto.Components);
                var notExisted = (await _unitOfWork.ComponentRepository.CheckExistence(
                      category.Components.Select((c) => c.ComponentTypeId).WhereNotNull()
                  )).Where((result) => !result.DoesExist);

                if (notExisted.Any())
                {
                    foreach (var notExistedComponent in notExisted)
                    {
                        ModelState.AddModelError(
                            $"components[{category.Components.FindIndex(c => c.ComponentTypeId == notExistedComponent.Id)}]",
                            $"Specified ComponentId({notExistedComponent.Id}) not refer to existing component"
                        );
                    }
                    var problem = new ValidationProblemDetails(ModelState);
                    return BadRequest(problem);
                }
            }

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.Complete();

            var categoryDto = await _unitOfWork.Categories.GetByIdProjectedAsync(category.CategoryId);

            using var categoryImageStream = createCategoryDto?.Image?.OpenReadStream();

            CategoryAssetHelper.Image = categoryImageStream is not null ? new AppFile()
            {
                File = categoryImageStream,
                FileName = createCategoryDto!.Image!.FileName,
                ContentType = createCategoryDto!.Image.ContentType
            } : null;

            using var categoryIconStream = createCategoryDto?.Icon?.OpenReadStream();

            CategoryAssetHelper.Icon = categoryIconStream is not null ? new AppFile
            {
                File = categoryIconStream,
                FileName = createCategoryDto!.Icon!.FileName,
                ContentType = createCategoryDto!.Icon!.ContentType
            } : null;

            await CategoryAssetHelper.Write(category.CategoryId);

            categoryDto!.Image = CategoryAssetHelper.GetIconPath(category.CategoryId);
            categoryDto.Icon = CategoryAssetHelper.GetIconPath(category.CategoryId);


            return CreatedAtAction(nameof(GetCategories), new { id = category.CategoryId }, categoryDto);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<CategoryDTO>> UpdateCategory(int id, [FromForm] CategoryUpdateDto categoryUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = await _unitOfWork.Categories.GetModelByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _mapper.Map(categoryUpdateDto, category);
            var categoryDto = _mapper.Map<CategoryDTO>(category);

            await CategoryAssetHelper.Write(category.CategoryId);
            categoryDto!.Image = CategoryAssetHelper.GetIconPath(category.CategoryId);
            categoryDto.Icon = CategoryAssetHelper.GetIconPath(category.CategoryId);

            await _unitOfWork.Complete();

            return Ok(categoryDto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<CategoryDTO>> DeleteCategory(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdProjectedAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            await _unitOfWork.Categories.Delete(category.CategoryId);
            await _unitOfWork.Complete();

            CategoryAssetHelper.RemoveAssets(id);

            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("{categoryId}/components")]
        [TypeFilter<ResourceExistenceActionFilterAttribute<Category>>]
        public async Task<ActionResult<ComponentType>> GetCategoryComponents(int categoryId)
        {
            var components = await _unitOfWork.ComponentRepository.GetComponentsOfCategory(categoryId);

            return Ok(components);
        }

        [HttpPost("{categoryId}/components")]
        [TypeFilter<ResourceExistenceActionFilterAttribute<Category>>]
        public async Task<ActionResult> CreateComponentForCategory([FromBody] ComponentCreationOrReferenceDto componentDto, int categoryId)
        {

            var category = await _unitOfWork.Categories.GetModelByIdAsync(categoryId);
            var componentType = _mapper.Map<CategoryComponent>(componentDto);

            if (componentType.ComponentTypeId is not null)
            {
                var isExisted = category!.Components?.Any(cmp => cmp.ComponentTypeId == componentType.ComponentTypeId) ?? false;
                // if component id specified then it tried to add existing component
                // and we must fetch other data
                if (isExisted)
                {
                    var component = await _unitOfWork.ComponentRepository.GetByIdProjectedAsync(componentDto.ComponentTypeId!.Value);
                    var problem = new ProblemDetails
                    {
                        Status = (int)HttpStatusCode.Conflict,
                        Title = "Duplicated model",
                        Detail = $"The component tried to insert for this category has existed before",
                        Extensions = {
                            ["component"] = component
                        }
                    };
                    return new BadRequestObjectResult(problem) { StatusCode = problem.Status };
                }
            }

            category!.Components!.Add(componentType);
            await _unitOfWork.Complete();

            return Ok(_mapper.Map<ComponentTypeDto>(componentType.Component));

        }
    }
}