using AutoMapper;

using HiTechStore.Core;
using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Category;
using HiTechStore.DTOs.Category;
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
            return Ok(categories);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDTO>> CreateCategory(CategoryCreationDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = new Category
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description,
                ParentCategoryId = categoryDto.ParentCategoryId
            };

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.Complete();

            return CreatedAtAction(nameof(GetCategories), new { id = category.CategoryId }, category);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<CategoryDTO>> UpdateCategory(int id, CategoryUpdateDto categoryDto)
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

            _mapper.Map(categoryDto, category);

            await _unitOfWork.Complete();

            return Ok(_mapper.Map<CategoryDTO>(category));
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