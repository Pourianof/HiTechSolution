
using System.Security.Claims;

using AutoMapper;

using HiTechStore.Controllers.ActionFilters;
using HiTechStore.Controllers.ExceptionFilters;
using HiTechStore.Core;
using HiTechStore.Core.Helpers;
using HiTechStore.Core.Services.Product;
using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Product;
using HiTechStore.Data.Mapping;
using HiTechStore.Data.Queries;
using HiTechStore.DTOs.Product;
using HiTechStore.Helpers.URLFilterQuery;
using HiTechStore.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;

        public ProductsController(IUnitOfWork unitOfWork, IMapper mapper, IProductService productService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _productService = productService;
        }

        [HttpGet]
        public async Task<PagedResultDto<ProductDto>> GetProducts([ToQuery] ProductQuery query)
        {
            var products = await _productService.GetProducts(query);

            return products;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
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
        [TypeFilter<ProductCreationActionFilterAttribute>]
        public IActionResult CreateProduct([FromForm] ProductCreationDto product)
        {
            var createdProductDto = HttpContext.Items["createdProductDto"] as ProductDto;
            if (createdProductDto?.ProductId is null)
            {
                return Problem(
                    title: "Product creation failed",
                    detail: "Something went wrong. product not created."
                );
            }

            return CreatedAtAction(nameof(GetProduct), new { id = createdProductDto.ProductId }, createdProductDto);
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
        [TypeFilter<HandleModelUpdateActionFilterAttribute<Product, ProductCreationDto>>]
        [TypeFilter<SameAuthorValidationActionFilterAttribute<Product>>]
        public IActionResult ReplaceProduct([FromBody] ProductCreationDto product)
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
            var product = await _unitOfWork.Products.GetModelByIdAsync(id);
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