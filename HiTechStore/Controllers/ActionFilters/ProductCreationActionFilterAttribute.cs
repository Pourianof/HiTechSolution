using HiTechStore.Core;
using HiTechStore.DTOs.Product;
using HiTechStore.Models;
using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using AutoMapper;
using HiTechStore.Helpers.IO;
using HiTechStore.Data.DTOs.Product;

namespace HiTechStore.Controllers.ActionFilters;

public class ProductCreationActionFilterAttribute : ModelAccessorBaseActionFilterAttribute<Product>
{
    private IMapper _mapper;
    public ProductCreationActionFilterAttribute(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
    {
        _mapper = mapper;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);

        ProductCreationDto? product = context.ActionArguments["product"] as ProductCreationDto;

        if (product == null)
        {
            var problem = new ValidationProblemDetails
            {
                Detail = "no product model defined",
                Title = "Bad request",
                Status = StatusCodes.Status400BadRequest
            };
            context.Result = new BadRequestObjectResult(problem);
            return;
        }

        var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            context.Result = new UnauthorizedObjectResult("You are not authorized to create a product.");
            return;
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

        var productMedia = product.Media!;
        DbSet.AddAsync(createdProduct).Wait();
        CompleteDbWork().Wait();

        try
        {
            var isMainSpecified = false;
            for (int index = 0; index < productMedia.Count(); index++)
            {
                var media = productMedia.ElementAt(index);
                var isImage = MediaTypeHelper.IsImage(media.FileName);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(media.FileName);
                string fileRelativePath = $"images/products/{createdProduct.ProductId}/${fileName}";
                PublicAssetsHelper.WriteIFormFile(media, fileRelativePath).Wait();
                bool isMain = product.MediaMetaData is null && isImage && !isMainSpecified ? true : index == product.MediaMetaData?.MainIndex;
                if (isMain) isMainSpecified = true;
                createdProduct.Media.Add(new ProductMedia { FilePath = $"/{fileRelativePath}", IsMain = isMain, Type = MediaTypeHelper.GetMediaType(fileRelativePath), });

            }
        }
        catch
        {
            DbSet.Delete(createdProduct);
            throw;
        }
        // For updating the product media
        CompleteDbWork().Wait();



        context.HttpContext.Items["createdProductDto"] = _mapper.Map<ProductDto>(createdProduct);
    }
}
