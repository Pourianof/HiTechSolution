using AutoMapper;

using HiTechStore.Core;
using HiTechStore.Core.Exceptions;
using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Category;
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

        private string ProvideCategoryImagePublicPath(int categoryId)
        {
            return Path.Combine("images", "category", $"{categoryId}.png");
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
                    var pubPath = ProvideCategoryImagePublicPath(cat.CategoryId);
                    categoryDto.Image = PublicAssetsHelper.IsExist(pubPath) ? pubPath : null;
                    return categoryDto;
                }
            );
            return Ok(categoryDtos);
        }

        private async Task<string> WriteCategoryImage(Category category, IFormFile image, bool deleteOnError = true)
        {
            var publicPath = ProvideCategoryImagePublicPath(category.CategoryId);
            try
            {
                await PublicAssetsHelper.WriteIFormFile(image, publicPath);
            }
            catch (SavingFileException)
            {
                if (deleteOnError)
                {
                    await _unitOfWork.Categories.Delete(category);
                }
                throw;
            }
            return publicPath;
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
                ParentCategoryId = createCategoryDto.ParentCategoryId
            };

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.Complete();

            var categoryDto = _mapper.Map<CategoryDTO>(category);

            if (createCategoryDto.Image is not null)
            {
                var imagePath = await WriteCategoryImage(category, createCategoryDto.Image);
                categoryDto.Image = imagePath;
            }


            return CreatedAtAction(nameof(GetCategories), new { id = category.CategoryId }, categoryDto);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<CategoryDTO>> UpdateCategory(int id, [FromForm] CategoryUpdateDto categoryUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _mapper.Map(categoryUpdateDto, category);
            var categoryDto = _mapper.Map<CategoryDTO>(category);

            if (categoryUpdateDto.Image is not null)
            {
                var imagePath = await WriteCategoryImage(category, categoryUpdateDto.Image, false);
                categoryDto.Image = imagePath;
            }


            await _unitOfWork.Complete();

            return Ok(_mapper.Map<CategoryDTO>(categoryDto));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<CategoryDTO>> DeleteCategory(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            await _unitOfWork.Categories.Delete(category);
            await _unitOfWork.Complete();

            return NoContent();
        }
    }
}