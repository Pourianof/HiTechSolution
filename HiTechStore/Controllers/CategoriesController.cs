using System.Net;

using AutoMapper;

using HiTechStore.Controllers.ActionFilters;
using HiTechStore.Core;
using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Category;
using HiTechStore.Data.DTOs.Component;
using HiTechStore.DTOs.Category;
using HiTechStore.Helpers.IO;
using HiTechStore.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = $"{IdentityRoles.Admin},{IdentityRoles.Manager}")]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoriesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            var categoryDtos = categories.Select(
                (cat) =>
                {
                    var categoryDto = _mapper.Map<CategoryDTO>(cat);
                    categoryDto.Image = CategoryAssetHelper.GetCategoryImagePathIfExist(cat.CategoryId);
                    categoryDto.Image = CategoryAssetHelper.GetCategoryIconPathIfExist(cat.CategoryId);
                    return categoryDto;
                }
            );
            return Ok(categoryDtos);
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
            }

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.Complete();

            var categoryDto = _mapper.Map<CategoryDTO>(category);

            var writer = new CategoryAssetHelper(_unitOfWork, category.CategoryId)
            {
                Icon = createCategoryDto.Icon,
                Image = createCategoryDto.Image
            };

            await writer.Write();

            categoryDto.Image = writer.ImagePath;
            categoryDto.Icon = writer.IconPath;


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

            var writer = new CategoryAssetHelper(_unitOfWork, category.CategoryId)
            {
                Image = categoryUpdateDto.Image,
                Icon = categoryUpdateDto.Icon,
                DeleteOnError = false
            };
            await writer.Write();
            categoryDto.Image = writer.ImagePath;
            categoryDto.Icon = writer.IconPath;



            await _unitOfWork.Complete();

            return Ok(categoryDto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<CategoryDTO>> DeleteCategory(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
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
        public async Task<ActionResult> CreateComponentForCategory([FromBody] ComponentCreationDto componentDto, int categoryId)
        {

            var category = await _unitOfWork.Categories.GetModelByIdAsync(categoryId);
            var componentType = _mapper.Map<CategoryComponent>(componentDto);

            if (componentType.ComponentId is not null)
            {
                var isExisted = category!.Components?.Any(cmp => cmp.ComponentId == componentType.ComponentId) ?? false;
                // if component id specified then it tried to add existing component
                // and we must fetch other data
                if (isExisted)
                {
                    var component = await _unitOfWork.ComponentRepository.GetByIdAsync(componentDto.ComponentId!.Value);
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