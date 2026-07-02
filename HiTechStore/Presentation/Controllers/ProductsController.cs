
using System.Security.Claims;

using AutoMapper;

using HiTechStore.Presentation.Controllers.ExceptionFilters;
using HiTechStore.Core.Dto.Product;
using HiTechStore.Core.Services.Product;
using HiTechStore.Infrastructure.Data.DTOs;
using HiTechStore.Infrastructure.Data.DTOs.Product;
using HiTechStore.Infrastructure.Data.Queries;
using HiTechStore.Helpers.URLFilterQuery;
using HiTechStore.Core.Models;
using HiTechStore.Presentation.Product;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HiTechStore.Presentation.Requests;
using HiTechStore.Core.Common.Interfaces.Infra;

namespace HiTechStore.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProductService _productService;

        public ProductsController(IMapper mapper, IProductService productService)
        {
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
        public async Task<ActionResult<ProductDto>> GetProduct(int id, [ToQuery] ProductQuery query)
        {
            var product = await _productService.GetProductById(id, new()
            {
                UsersScore = true,
                DiscountCalculation = true
            }, query);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        [HttpGet("{id}/similars")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetSimilarProducts(int id, [ToQuery] ProductQuery? productQuery)
        {
            var products = await _productService.GetSimilarProductsOf(id, productQuery);

            return Ok(products);
        }

        [HttpGet("on-sales")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetOnSaleProducts([ToQuery] ProductQuery? productQuery)
        {
            var onSaleProducts = await _productService.GetOnSaleProducts(productQuery);

            return Ok(
                onSaleProducts
            );
        }


        [HttpPost]
        [ViolateForeignKeyExceptionFilter]
        public async Task<IActionResult> CreateProduct([FromForm] ProductCreationRequest product)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var productCreationDto = _mapper.Map<ProductCreationDto>(product);

            productCreationDto.Media = product.Media!.Select(
                (m) => new AppFile
                {
                    File = m.OpenReadStream(),
                    FileName = m.FileName,
                    ContentType = m.ContentType
                }
            );
            productCreationDto.Thumbnails = product.Thumbnails?.Select(
                (thumb) => new AppFile
                {
                    File = thumb.OpenReadStream(),
                    FileName = thumb.FileName,
                    ContentType = thumb.ContentType
                }
            );

            var createdProductDto = await _productService.CreateProduct(productCreationDto, userId!);

            foreach (var m in productCreationDto.Media)
            {
                await m.File.DisposeAsync();
            }

            foreach (var thumb in productCreationDto.Thumbnails ?? [])
            {
                await thumb.File.DisposeAsync();
            }

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

        [HttpPut("{id}/category")]
        public async Task<ActionResult<ProductDto>> UpdateProductCategory(int id, ProductCategoryValuesRequest categoryReplaceRequest)
        {
            var product = await _productService.UpdateProductsCategory(id, _mapper.Map<ProductCategoryValuesDto>(categoryReplaceRequest));

            return product;
        }

    }
}