
using System.Security.Claims;

using AutoMapper;

using HiTechStore.Controllers.ActionFilters;
using HiTechStore.Controllers.ExceptionFilters;
using HiTechStore.Core;
using HiTechStore.Data.DTOs.Product;
using HiTechStore.DTOs.Product;
using HiTechStore.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public Task<IEnumerable<Product>> GetProducts()
        {
            return _unitOfWork.Products.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var product = await _unitOfWork.Products.GetByIdAsync(id, userId);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        [HttpPost]
        [Authorize(Roles = $"{IdentityRoles.Admin},{IdentityRoles.Manager}")]
        [ViolateForeignKeyExceptionFilter]
        public IActionResult CreateProduct([FromBody] ProductDTO product)
        {
            if (product == null)
            {
                return BadRequest();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("You are not authorized to create a product.");
            }

            var createdProduct = _mapper.Map<Product>(product);

            if (product.Categories is not null)
            {

                createdProduct.Categories = product.Categories.Select(c => new ProductCategory
                {
                    CategoryId = c
                }).ToList();

            }

            createdProduct.AuthorId = userId;

            _unitOfWork.Products.AddAsync(createdProduct).Wait();
            _unitOfWork.Complete().Wait();

            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.ProductId }, createdProduct);
        }

        [HttpPatch("{id}")]
        [TypeFilter<HandleModelUpdateActionFilterAttribute<Product, ProductPatchDTO>>]
        [TypeFilter<SameAuthorValidationActionFilterAttribute<Product>>]
        public IActionResult UpdateProduct(int id, [FromBody] ProductPatchDTO product)
        {
            var actualProduct = HttpContext.Items["model"] as Product;
            if (actualProduct == null)
            {
                return NotFound();
            }

            _mapper.Map(product, actualProduct);

            _unitOfWork.Complete();
            _mapper.Map(actualProduct, product);
            return Ok(product);
        }

        [HttpPut("{id}")]
        [TypeFilter<HandleModelUpdateActionFilterAttribute<Product, ProductDTO>>]
        [TypeFilter<SameAuthorValidationActionFilterAttribute<Product>>]
        public IActionResult ReplaceProduct([FromBody] ProductDTO product)
        {
            var actualProduct = HttpContext.Items["model"] as Product;
            if (actualProduct == null)
            {
                return NotFound();
            }

            _mapper.Map(product, actualProduct);

            _unitOfWork.Complete();
            return Ok(product);
        }

        [HttpDelete("{id}")]
        [TypeFilter<SameAuthorValidationActionFilterAttribute<Product>>]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                return BadRequest();
            }

            await _unitOfWork.Products.Delete(product);
            await _unitOfWork.Complete();

            return NoContent();
        }

        [HttpPost("{productId}/score/me")]
        [Authorize]
        public async Task<IActionResult> ScoreProduct(int productId, [FromBody] ProductScoreDto score)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized();
            }
            // check if is any score registered by this user for this product before
            var existingScore = await _unitOfWork.ProductScores.GetUserScoreForProductAsync(userId, productId);
            if (existingScore != null)
            {
                // if exist delete it
                await _unitOfWork.ProductScores.Delete(existingScore);
            }

            // register new one
            var newScore = new ProductScore
            {
                UserId = userId,
                ProductId = productId,
                Score = score.Score // default score
            };
            await _unitOfWork.ProductScores.AddAsync(newScore);
            await _unitOfWork.Complete();

            return Ok(newScore);
        }

    }
}