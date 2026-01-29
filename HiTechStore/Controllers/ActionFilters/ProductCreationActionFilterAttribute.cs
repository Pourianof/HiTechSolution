using HiTechStore.Core;
using HiTechStore.DTOs.Product;
using HiTechStore.Models;
using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using AutoMapper;
using HiTechStore.Helpers.IO;
using HiTechStore.Data.DTOs.Product;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HiTechStore.Helpers.Types;
using HiTechStore.Core.Exceptions;
using HiTechStore.Core.Repositories;

namespace HiTechStore.Controllers.ActionFilters;

public class ProductCreationActionFilterAttribute : ModelAccessorBaseActionFilterAttribute<Product>
{
    private IMapper _mapper;
    public ProductCreationActionFilterAttribute(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
    {
        _mapper = mapper;
    }

    protected ObjectResult InvalidModelState(ModelStateDictionary modelState)
    {
        var problem = new ValidationProblemDetails(modelState);

        return new BadRequestObjectResult(problem);
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

        if (product.BrandModel is not null)
        {
            var brandModel = UnitOfWork.BrandModelRepository.GetModelByIdAsync(product.BrandModel.Value).Result;
            if (brandModel is null)
            {
                var problem = new ProblemDetails
                {
                    Detail = $"Specified brandModel with id '{product.BrandModel}' not exist",
                    Title = "Bad request",
                    Status = StatusCodes.Status404NotFound
                };
                context.Result = new NotFoundObjectResult(problem);
                return;
            }

            createdProduct.BrandModel = brandModel;
        }

        // register product properties
        if (product.CategoryValues is not null)
        {

            // setting product category
            var categoryId = product.CategoryValues.CategoryId!.Value;
            createdProduct.CategoryId = categoryId;

            var productCategory = UnitOfWork.Categories.GetByIdProjectedAsync(categoryId).Result;

            if (productCategory is null)
            {
                var problem = new ProblemDetails()
                {
                    Title = "Category not found",
                    Detail = $"Category with id {categoryId} not exist",
                    Status = StatusCodes.Status404NotFound
                };
                context.Result = new NotFoundObjectResult(problem);
                return;
            }


            if (product.CategoryValues.ComponentModels != null && product.CategoryValues.ComponentModels.Any())
            {
                var componentModelIds = product.CategoryValues.ComponentModels;

                var categoryValidModels = UnitOfWork.Categories.GetModelsOfCategory(categoryId, componentModelIds).Result;

                for (var index = 0; index < componentModelIds.Count(); index++)
                {
                    var modelId = componentModelIds.ElementAt(index);
                    var componentModel = categoryValidModels.Where((cvm) => cvm.ComponentModelId == modelId).FirstOrDefault();

                    if (componentModel is null)
                    {
                        context.ModelState.AddModelError($"categoryValues.componentModels.{index}",
                            $"specified component model-id is not belong to a component of the '{productCategory.Name}' category");
                        var problem = new ValidationProblemDetails(context.ModelState)
                        {
                            Status = StatusCodes.Status400BadRequest
                        };
                        context.Result = new BadRequestObjectResult(problem);
                        return;
                    }

                    createdProduct.ComponentModels.Add(componentModel);
                }


            }

            var categoryProperties = productCategory.Properties;

            createdProduct.Properties = new List<ProductPropertyValue>();

            for (int index = 0; index < (product.CategoryValues.Properties?.Count() ?? 0); index++)
            {
                var prop = product.CategoryValues.Properties!.ElementAt(index);

                // some value must specified for property
                if (prop.PropertyValue is null)
                {
                    context.ModelState.AddModelError($"CategoryValues.Properties.{index}.PropertyValue", "PropertyValue is required");
                    context.Result = InvalidModelState(context.ModelState);
                    return;
                }

                var ppv = new ProductPropertyValue
                {
                    PropertyId = prop.PropertyId!.Value,

                };

                // get the property which this item target to
                var actualProp = categoryProperties?.FirstOrDefault((p) => p.PropertyId == prop.PropertyId);

                if (actualProp is null)
                {
                    context.ModelState.AddModelError($"CategoryValues.Properties.{index}.PropertyId", $"No PropertyId with id {prop.PropertyId} exist for specified category.");
                    context.Result = InvalidModelState(context.ModelState);
                    continue;
                }

                var pv = new PropertyValue();

                try
                {
                    pv.PopulateValue(actualProp.PropertyType!.Value, prop.PropertyValue);
                }
                catch (PropertyValueTypeDismatchException ex)
                {
                    context.ModelState.AddModelError(
                        $"CategoryValues.Properties.{index}.PropertyValue",
                        ex.Message
                    );
                    context.Result = InvalidModelState(context.ModelState);
                    return;
                }
                ppv.Value = pv;
                createdProduct.Properties.Append(ppv);
            }
        }


        createdProduct.AuthorId = userId;

        var productMedia = product.Media!;
        UnitOfWork.Products.AddAsync(createdProduct).Wait();
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
            UnitOfWork.Products.Delete(createdProduct);
            throw;
        }
        // For updating the product media
        CompleteDbWork().Wait();



        context.HttpContext.Items["createdProductDto"] = (Repo as IProductRepository)!.GetByIdProjectedAsync(createdProduct.ProductId).Result;
    }
}
