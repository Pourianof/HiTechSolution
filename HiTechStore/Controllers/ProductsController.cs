
using System.Security.Claims;
using System.Threading.Tasks;

using AutoMapper;

using HiTechStore.Controllers.ExceptionFilters;
using HiTechStore.Core;
using HiTechStore.Core.Services.Product;
using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Product;
using HiTechStore.Data.Queries;
using HiTechStore.DTOs.Product;
using HiTechStore.Helpers.URLFilterQuery;
using HiTechStore.Models;
using HiTechStore.Presentation.Product;

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
            var product = await _productService.GetProductById(id, new()
            {
                UsersScore = true,
                DiscountCalculation = true
            });

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        [HttpGet("{id}/similars")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetSimilarProducts(int id)
        {
            var products = await _productService.GetSimilarProductsOf(id);

            return Ok(products);
        }

        [HttpGet("on-sales")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetOnSaleProducts()
        {
            var onSaleProducts = await _productService.GetOnSaleProducts();

            return Ok(
                onSaleProducts
            );
        }


        [HttpPost]
        [Authorize(Roles = $"{IdentityRoles.Admin},{IdentityRoles.Manager}")]
        [ViolateForeignKeyExceptionFilter]
        public async Task<IActionResult> CreateProduct([FromForm] ProductCreationDto product)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var createdProductDto = await _productService.CreateProduct(product, userId!);
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
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateRequest updateRequest)
        {
            var result = await _productService.UpdateProduct(id, _mapper.Map<UpdateProductDto>(updateRequest));

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productService.DeleteProduct(id);
            if (product == null)
            {
                return BadRequest();
            }

            return NoContent();
        }

    }
}