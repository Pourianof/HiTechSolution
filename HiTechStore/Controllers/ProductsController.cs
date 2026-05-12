
using System.Security.Claims;

using AutoMapper;

using HiTechStore.Controllers.ActionFilters;
using HiTechStore.Controllers.ExceptionFilters;
using HiTechStore.Core;
using HiTechStore.Core.Services.Product;
using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Product;
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
            var product = await _productService.DeleteProduct(id);
            if (product == null)
            {
                return BadRequest();
            }

            return NoContent();
        }

    }
}