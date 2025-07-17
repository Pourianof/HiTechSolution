
using AutoMapper;

using HiTechStore.Controllers.ActionFilters;
using HiTechStore.Core;
using HiTechStore.DTOs.Product;
using HiTechStore.Models;

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
            var product = await _unitOfWork.Products.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        [HttpPost]
        public IActionResult CreateProduct([FromBody] ProductDTO product)
        {
            if (product == null)
            {
                return BadRequest();
            }

            var createdProduct = _mapper.Map<Product>(product);

            _unitOfWork.Products.AddAsync(createdProduct).Wait();
            _unitOfWork.Complete().Wait();

            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.ProductId }, createdProduct);
        }

        [HttpPatch("{id}")]
        [TypeFilter<HandleModelUpdateActionFilterAttribute<Product, ProductPatchDTO>>]
        public IActionResult UpdateProduct(int id, [FromBody] ProductPatchDTO product)
        {
            var actualProduct = HttpContext.Items["model"] as Product;
            if (actualProduct == null)
            {
                return NotFound();
            }

            _mapper.Map(product, actualProduct);

            _unitOfWork.Complete();
            return Ok(actualProduct);
        }

        [HttpPut("{id}")]
        [TypeFilter<HandleModelUpdateActionFilterAttribute<Product, ProductDTO>>]
        public IActionResult ReplaceProduct([FromBody] ProductDTO product)
        {
            var actualProduct = HttpContext.Items["model"] as Product;
            if (actualProduct == null)
            {
                return NotFound();
            }

            _mapper.Map(product, actualProduct);

            _unitOfWork.Complete();
            return Ok(actualProduct);
        }

        [HttpDelete("{id}")]
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

    }
}